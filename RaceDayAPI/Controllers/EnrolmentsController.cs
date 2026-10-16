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
    public class EnrolmentsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;
        public EnrolmentsController(RaceDayDbContext context) { _context = context; }

        [HttpPost("events/{eventId}/enrol")]
        [RequireRole("Participant")]
        public async Task<IActionResult> Enrol(int eventId, [FromBody] EnrolmentRequestDto dto)
        {
            var userId = HttpContext.Session.GetInt("UserId");
            if (userId == null) return Unauthorized("Not logged in.");

            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.UserID == userId.Value);
            if (participant == null) return NotFound("Participant profile not found.");

            var alreadyEnrolled = await _context.Enrolments.AnyAsync(e => e.ParticipantID == participant.ParticipantID && e.EventID == eventId);
            if (alreadyEnrolled) return Conflict("Already enrolled in this event.");

            var enrolment = new Enrolment
            {
                ParticipantID = participant.ParticipantID,
                EventID = eventId,
                CategoryID = dto.CategoryId,
                Status = "Pending",
                EnrolmentDate = DateTime.Now
            };
            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();
            return Ok("Enrolled successfully.");
        }

        [HttpGet("events/{eventId}/enrolments")]
        [RequireRole("Organiser")]
        public async Task<IActionResult> GetEventEnrolments(int eventId)
        {
            var enrolments = await _context.Enrolments
                .Include(e => e.Participant)
                .Include(e => e.Category)
                .Where(e => e.EventID == eventId).ToListAsync();
            return Ok(enrolments);
        }

        [HttpGet("my-enrolments")]
        [RequireRole("Participant")]
        public async Task<IActionResult> GetMyEnrolments()
        {
            var userId = HttpContext.Session.GetInt("UserId");
            if (userId == null) return Unauthorized("Not logged in.");

            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.UserID == userId.Value);
            if (participant == null) return NotFound("Participant profile not found.");

            var enrolments = await _context.Enrolments
                .Include(e => e.Event)
                .Include(e => e.Category)
                .Where(e => e.ParticipantID == participant.ParticipantID).ToListAsync();

            return Ok(enrolments);
        }
    }
}