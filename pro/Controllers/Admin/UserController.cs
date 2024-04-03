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
                .ThenInclude(su => su.Skill) // Include Skill entity in SkillUsers
                .Include(u => u.DepartmentUsers)
                .ThenInclude(du => du.Department) // Include Department entity in DepartmentUsers
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
                ApplicantDetails = user.Applicant, // Assuming Applicant is a navigation property in User
                JobApplicationDetails = user.JobApplication, // Assuming JobApplication is a navigation property in User
                AcknowledgmentDetails = user.Acknowledgment, // Assuming Acknowledgment is a navigation property in User
                Educations = user.Educations, // Assuming Educations is a navigation property in User
                SkillDetails = user.SkillUsers.Select(skillUser => new
                {
                    SkillName = skillUser.Skill?.SkillName,
                    SkillType = skillUser.Skill?.SkillType
                }),
                DepartmentDetails = user.DepartmentUsers.Select(du => new
                {
                    DepartmentId = du.DepartmentID,
                    DepartmentName = du.Department?.DepartmentName // Assuming DepartmentName is a property in Department
                }),
                CompanyDetails = user.Company, // Assuming Company is a navigation property in User
                FileUploadResponses = user.FileUploadResponses // Assuming FileUploadResponses is a navigation property in User
            });

            return Ok(userDetails);
        }

    }
}
