using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using pro.Data;
using pro.DTOs.Inside;
using pro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace pro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EducationController : ControllerBase
    {
        private readonly Context _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<EducationController> _logger;

        public EducationController(Context context, UserManager<User> userManager, ILogger<EducationController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost("app")]
        public async Task<ActionResult<Education>> CreateApplicantForUser(Edu edu)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Remove existing education, skill users, and department user records for the user
            var existingEducation = _context.Educations.FirstOrDefault(e => e.UserId == userId);
            if (existingEducation != null)
            {
                _context.Educations.Remove(existingEducation);

                var existingSkillUsers = _context.SkillUsers.Where(s => s.UserId == userId);
                _context.SkillUsers.RemoveRange(existingSkillUsers);

                var existingDepartmentUser = _context.DepartmentUsers.FirstOrDefault(d => d.UserId == userId);
                if (existingDepartmentUser != null)
                {
                    _context.DepartmentUsers.Remove(existingDepartmentUser);
                }

                await _context.SaveChangesAsync();
            }

            // Create new education record
            var newEducation = new Education
            {
                UserId = userId,
                CurrentStatus = edu.EducationDTO.CurrentStatus,
                Qulification = edu.EducationDTO.Qulification,
                InsituteName = edu.EducationDTO.InsituteName,
                Yearattained = edu.EducationDTO.Yearattained,
            };

            var softSkillIds = GetSkillIdsByNames(edu.SkillUserDTO.SoftSkill);
            var hardSkillIds = GetSkillIdsByNames(edu.SkillUserDTO.HardSkill);
            var languageIds = GetSkillIdsByNames(edu.SkillUserDTO.Language);

            // Create new skill user records
            var skillUsers = new List<SkillUser>();
            foreach (var skillId in softSkillIds.Concat(hardSkillIds).Concat(languageIds))
            {
                var newSkillUser = new SkillUser
                {
                    UserId = userId,
                    Skillid = skillId,
                };
                skillUsers.Add(newSkillUser);
            }

            // Create new department user record
            var departmentId = GetDepartmentIdByName(edu.DepartmentUserDTO.DepartmentID);
            var newDepartmentUser = new DepartmentUser
            {
                UserId = userId,
                DepartmentID = departmentId,
            };

            // Add new records to the context and save changes
            _context.SkillUsers.AddRange(skillUsers);
            _context.DepartmentUsers.Add(newDepartmentUser);
            _context.Educations.Add(newEducation);
            await _context.SaveChangesAsync();

            return Ok(newEducation);
        }

        [HttpGet("user-details")]
        [Authorize]
        public async Task<ActionResult<UserDetails>> GetUserDetails()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var userDetails = new UserDetails
                {
                    UserId = userId,
                    UserName = user.UserName,
                    // Fetch education details
                    Education = await _context.Educations.FirstOrDefaultAsync(e => e.UserId == userId),
                    // Fetch skill details
                    Skills = await _context.SkillUsers
                        .Where(s => s.UserId == userId)
                        .Select(s => s.Skill)
                        .ToListAsync(),
                    Department = await _context.DepartmentUsers
                        .Where(d => d.UserId == userId)
                        .Select(d => d.Department)
                        .FirstOrDefaultAsync()
                };

                return Ok(userDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user details.");
                return StatusCode(500, "Internal server error");
            }
        }

        private List<int> GetSkillIdsByNames(List<int> skillNames)
        {
            return _context.Skills
                .Where(skill => skillNames.Contains(skill.Skillid))
                .Select(skill => skill.Skillid)
                .ToList();
        }

        private int GetDepartmentIdByName(int departmentId)
        {
            var department = _context.Departments.FirstOrDefault(d => d.DepartmentID == departmentId);
            return department?.DepartmentID ?? 0; // Return 0 if not found or actual department ID
        }
    }
}
