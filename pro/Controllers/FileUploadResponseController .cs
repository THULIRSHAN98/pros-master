using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using pro.Data;
using pro.Models;
using pro.DTOs;
using Microsoft.EntityFrameworkCore;

namespace pro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileUploadResponseController : ControllerBase
    {
        private readonly Context _context; // Database access
        private readonly UserManager<User> _userManager; // Register user

        public FileUploadResponseController(Context context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        [HttpPost("upload")]
        public async Task<ActionResult<FileUploadResponse>> UploadFile(FileUploadRequestDTO fileUploadRequest)
        {
            try
            {
                // Ensure the ModelState is valid
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var existingFileUploadResponse = await _context.FileUploadResponses.FirstOrDefaultAsync(f => f.UserId == userId);

                if (existingFileUploadResponse != null)
                {
                    // Remove the existing file upload response from the database
                    _context.FileUploadResponses.Remove(existingFileUploadResponse);
                }

                // Create a new file upload response
                var newFileUploadResponse = new FileUploadResponse
                {
                    FileName = fileUploadRequest.FileName,
                    FilePath = fileUploadRequest.FilePath,
                    FileSize = fileUploadRequest.FileSize,
                    ContentType = fileUploadRequest.ContentType,
                    Status = fileUploadRequest.Status, // Set the status based on user input
                    User = user // Assign the user to the file upload response
                };

                // Add the new file upload response to the database
                _context.FileUploadResponses.Add(newFileUploadResponse);

                await _context.SaveChangesAsync();

                return Ok(newFileUploadResponse);
            }
            catch (Exception ex)
            {
                // Log error
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("file")]
        public async Task<ActionResult<FileUploadResponse>> GetFile()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var fileUploadResponse = await _context.FileUploadResponses.FirstOrDefaultAsync(f => f.UserId == userId);

                if (fileUploadResponse == null)
                {
                    return NotFound("No file uploaded for the user.");
                }

                return Ok(fileUploadResponse);
            }
            catch (Exception ex)
            {
                // Log error
                return StatusCode(500, "Internal Server Error");
            }
        }

    }
}
