using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using pro.Models;
using pro.DTOs;
using pro.Data;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace pro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicantController : ControllerBase
    {
        private readonly Context _context; //database access
        private readonly UserManager<User> _userManager; //register user

        public ApplicantController(Context context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        [HttpPost("app")]
        public async Task<ActionResult<Applicant>> CreateApplicantForUser(ApplicantDTO applicantDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                // Find existing applicant data associated with the user
                var existingApplicant = await _context.Applicants.FirstOrDefaultAsync(a => a.UserId == userId);

                if (existingApplicant != null)
                {
                    // Remove existing applicant data
                    _context.Applicants.Remove(existingApplicant);
                }

                var newApplicant = new Applicant
                {
                    UserId = userId,
                    Title = applicantDto.Title,
                    Dob = applicantDto.Dob,
                    Gender = applicantDto.Gender,
                    PhoneNo = applicantDto.PhoneNo,
                    Email = applicantDto.Email,
                    Address = applicantDto.Address,
                    Street = applicantDto.Street,
                    City = applicantDto.City,
                    State = applicantDto.State,
                    Zip = applicantDto.Zip,
                    Country = applicantDto.Country,
                };

                // Add new applicant data
                _context.Applicants.Add(newApplicant);

                await _context.SaveChangesAsync();

                return Ok(newApplicant);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Duplicate entry");
            }
        }
        [Authorize]
        [HttpGet("app")]
        public async Task<ActionResult<Applicant>> GetApplicantForUser()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var applicant = await _context.Applicants.FirstOrDefaultAsync(a => a.UserId == userId);

                if (applicant == null)
                {
                    return NotFound("Applicant data not found for the user.");
                }

                return Ok(applicant);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize]
        [HttpPut("app")]
        public async Task<ActionResult<Applicant>> UpdateApplicantForUser(ApplicantDTO applicantDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var applicant = await _context.Applicants.FirstOrDefaultAsync(a => a.UserId == userId);

                if (applicant == null)
                {
                    return NotFound("Applicant data not found for the user.");
                }

                // Update applicant data
                applicant.Title = applicantDto.Title;
                applicant.Dob = applicantDto.Dob;
                applicant.Gender = applicantDto.Gender;
                applicant.PhoneNo = applicantDto.PhoneNo;
                applicant.Email = applicantDto.Email;
                applicant.Address = applicantDto.Address;
                applicant.Street = applicantDto.Street;
                applicant.City = applicantDto.City;
                applicant.State = applicantDto.State;
                applicant.Zip = applicantDto.Zip;
                applicant.Country = applicantDto.Country;

                _context.Update(applicant);
                await _context.SaveChangesAsync();

                return Ok(applicant);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Applicant>>> GetAllApplicants()
        {
            try
            {
                var applicants = await _context.Applicants.ToListAsync();

                if (applicants == null || applicants.Count == 0)
                {
                    return NotFound("No applicants found.");
                }

                return Ok(applicants);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }



    }
}
