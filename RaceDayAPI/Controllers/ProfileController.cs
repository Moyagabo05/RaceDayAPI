using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using RaceDayAPI.Helpers;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ProfileController(RaceDayDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [RequireAuth]
        public async Task<IActionResult> GetProfile()
        {
            var userId = HttpContext.Session.GetInt("UserId");
            var role = HttpContext.Session.GetString("UserRole");

            if (userId == null) return Unauthorized("Not logged in.");

            if (role == "Organiser")
            {
                var organiser = await _context.Organisers.FirstOrDefaultAsync(o => o.UserID == userId.Value);
                if (organiser == null) return NotFound("Organiser profile not found.");

                return Ok(new
                {
                    organiser.UserID,
                    organiser.FirstName,
                    organiser.LastName,
                    organiser.OrganisationName,
                    organiser.PhoneNumber
                });
            }
            else if (role == "Participant")
            {
                var participant = await _context.Participants.FirstOrDefaultAsync(p => p.UserID == userId.Value);
                if (participant == null) return NotFound("Participant profile not found.");

                return Ok(new
                {
                    participant.UserID,
                    participant.FirstName,
                    participant.LastName,
                    participant.DateOfBirth,
                    participant.Gender,
                    participant.EmergencyContact,
                    participant.ClubName
                });
            }

            return BadRequest("Invalid role.");
        }

        [HttpPut]
        [RequireAuth]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userId = HttpContext.Session.GetInt("UserId");
            var role = HttpContext.Session.GetString("UserRole");

            if (userId == null) return Unauthorized("Not logged in.");

            if (role == "Organiser")
            {
                var organiser = await _context.Organisers.FirstOrDefaultAsync(o => o.UserID == userId.Value);
                if (organiser == null) return NotFound("Organiser profile not found.");

                organiser.FirstName = dto.FirstName;
                organiser.LastName = dto.LastName;
                organiser.OrganisationName = dto.OrganisationName;
                organiser.PhoneNumber = dto.PhoneNumber;
            }
            else if (role == "Participant")
            {
                var participant = await _context.Participants.FirstOrDefaultAsync(p => p.UserID == userId.Value);
                if (participant == null) return NotFound("Participant profile not found.");

                participant.FirstName = dto.FirstName;
                participant.LastName = dto.LastName;
                participant.EmergencyContact = dto.EmergencyContact;
                participant.ClubName = dto.ClubName;
            }

            await _context.SaveChangesAsync();
            return Ok("Profile updated successfully.");
        }
    }
}