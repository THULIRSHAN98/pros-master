using Microsoft.AspNetCore.Mvc;
using pro.Models;
using pro.DTOs.Inside;
using pro.Data;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace pro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PositionController : ControllerBase
    {
        private readonly Context _context; 

        public PositionController(Context context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Position>> CreatePosition(PositionDTO positionDto)
        {
            try
            {
                var newPosition = new Position
                {
                    PositionName = positionDto.PositionName
                };

                _context.Positions.Add(newPosition);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetPosition), new { id = newPosition.Id }, newPosition);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Position>> GetPosition(int id)
        {
            try
            {
                var position = await _context.Positions.FindAsync(id);

                if (position == null)
                {
                    return NotFound();
                }

                return Ok(position);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
