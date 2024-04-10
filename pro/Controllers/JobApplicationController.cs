using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using pro.Data;
using pro.DTOs.Inside;
using pro.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq;


namespace pro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobApplicationController : ControllerBase
    {
        private readonly Context _context;
        private readonly UserManager<User> _userManager;

        public JobApplicationController(Context context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        [HttpPost("Job")]
        public async Task<ActionResult<JobApplication>> CreateJobApplication(JobApplicationDTO jobApplicationDTO)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            // Remove any existing job applications for the user
            var existingJobApplications = await _context.JobApplications.Where(j => j.UserId == userId).ToListAsync();
            foreach (var existingJobApplication in existingJobApplications)
            {
                _context.JobApplications.Remove(existingJobApplication);
            }


            // Create a new job application
            var newJobApplication = new JobApplication
            {
                UserId = userId,
                DesiredLocation = jobApplicationDTO.DesiredLocation,
                IsFullTimePosition = jobApplicationDTO.IsFullTimePosition,
                StartDate = jobApplicationDTO.StartDate,
                Source = jobApplicationDTO.Source,
                PreferredContactMethod = jobApplicationDTO.PreferredContactMethod,
            };

            _context.JobApplications.Add(newJobApplication);
            await _context.SaveChangesAsync();

            return Ok(newJobApplication);
        }
    }
}
