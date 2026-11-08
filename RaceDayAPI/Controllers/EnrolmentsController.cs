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
            var userId = HttpContext.GetUserId();
            if (userId == null) return Unauthorized("Not logged in.");

            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.UserID == userId.Value);
            if (participant == null) return NotFound("Participant profile not found.");

            if (!await _context.Events.AnyAsync(e => e.EventID == eventId))
                return NotFound("Event not found.");

            if (!await _context.Categories.AnyAsync(c => c.CategoryID == dto.CategoryId && c.EventID == eventId))
                return BadRequest("The selected category does not belong to this event.");

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
            var userId = HttpContext.GetUserId();
            if (userId == null) return Unauthorized("Not logged in.");

            var ownsEvent = await _context.Events.AnyAsync(e => e.EventID == eventId && e.Organiser.UserID == userId.Value);
            if (!ownsEvent) return Forbid();

            var enrolments = await _context.Enrolments
                .Where(e => e.EventID == eventId)
                .Select(e => new EnrolmentDto
                {
                    EnrolmentID = e.EnrolmentID,
                    ParticipantID = e.ParticipantID,
                    ParticipantName = e.Participant.FirstName + " " + e.Participant.LastName,
                    EventID = e.EventID,
                    EventName = e.Event.Name,
                    CategoryID = e.CategoryID,
                    CategoryName = e.Category.CategoryName,
                    Status = e.Status,
                    EnrolmentDate = e.EnrolmentDate
                }).ToListAsync();
            return Ok(enrolments);
        }

        [HttpGet("my-enrolments")]
        [RequireRole("Participant")]
        public async Task<IActionResult> GetMyEnrolments()
        {
            var userId = HttpContext.GetUserId();
            if (userId == null) return Unauthorized("Not logged in.");

            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.UserID == userId.Value);
            if (participant == null) return NotFound("Participant profile not found.");

            var enrolments = await _context.Enrolments
                .Where(e => e.ParticipantID == participant.ParticipantID)
                .Select(e => new
                {
                    e.EnrolmentID,
                    EventName = e.Event.Name,
                    e.Event.EventDate,
                    e.Event.BannerImageUrl,
                    CategoryName = e.Category.CategoryName,
                    e.Status
                }).ToListAsync();

            return Ok(enrolments);
        }
    }
}