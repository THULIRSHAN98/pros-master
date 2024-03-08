using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pro.Data;
using pro.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Collections.Generic;

namespace pro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authorization for all endpoints in this controller
    public class AcknowledgmentController : ControllerBase
    {
        private readonly Context _context;
        private readonly UserManager<User> _userManager;

        public AcknowledgmentController(Context context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<ActionResult<Acknowledgment>> CreateAcknowledgment(Acknowledgment acknowledgment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Get authenticated user's ID
            var user = await _userManager.FindByIdAsync(userId); // Get user from UserManager

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Check if the user already has an acknowledgment
            var existingAcknowledgment = await _context.Acknowledgments.FirstOrDefaultAsync(a => a.UserId == userId);

            if (existingAcknowledgment != null)
            {
                // Remove existing acknowledgment
                _context.Acknowledgments.Remove(existingAcknowledgment);
            }

            // Associate acknowledgment with the authenticated user
            acknowledgment.UserId = userId;
            _context.Acknowledgments.Add(acknowledgment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMyAcknowledgment), null, acknowledgment);
        }


        [HttpGet("me")]
        public async Task<ActionResult<Acknowledgment>> GetMyAcknowledgment()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Get authenticated user's ID
            var acknowledgment = await _context.Acknowledgments.FirstOrDefaultAsync(a => a.UserId == userId);

            if (acknowledgment == null)
            {
                return NotFound("Acknowledgment not found.");
            }

            return acknowledgment;
        }

    }
}
