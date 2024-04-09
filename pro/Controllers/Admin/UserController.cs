using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using pro.Models;
using pro.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace pro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly Context _context;

        public UserController(UserManager<User> userManager, Context context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet("user")]
        public async Task<IActionResult> UserDetails()
        {
            var users = await _context.Users
                .Include(u => u.Applicant)
                .Include(u => u.JobApplication)
                .Include(u => u.Acknowledgment)
                .Include(u => u.Educations)
                .Include(u => u.SkillUsers)
                .ThenInclude(su => su.Skill)
                .Include(u => u.DepartmentUsers)
                .ThenInclude(du => du.Department)
                .Include(u => u.Company)
                .Include(u => u.FileUploadResponses)
                .ToListAsync();

            var userDetails = users.Select(user => new
            {
                user.Id,
                user.UserName,
                user.FirstName,
                user.LastName,
                user.DateCreated,
                user.Status,
                ApplicantDetails = user.Applicant,
                JobApplicationDetails = user.JobApplication,
                AcknowledgmentDetails = user.Acknowledgment,
                Educations = user.Educations,
                SkillDetails = user.SkillUsers.Select(skillUser => new
                {
                    SkillName = skillUser.Skill?.SkillName,
                    SkillType = skillUser.Skill?.SkillType
                }),
                DepartmentDetails = user.DepartmentUsers.Select(du => new
                {
                    DepartmentId = du.DepartmentID,
                    DepartmentName = du.Department?.DepartmentName
                }),
                CompanyDetails = user.Company,
                FileUploadResponses = user.FileUploadResponses,
            });

            return Ok(userDetails);
        }

    
    }
}
