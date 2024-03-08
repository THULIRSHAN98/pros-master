using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using pro.Models;
using pro.Data;
using Microsoft.EntityFrameworkCore;

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
                .Include(u => u.DepartmentUsers)
                .Include(u => u.Company)
                .Include(u => u.FileUploadResponses)
                .ToListAsync();

            return Ok(users);
        }
    }
}
