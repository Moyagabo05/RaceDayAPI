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

        // Participant enrolls in an event
        [HttpPost("events/{eventId}/enrol")]
        //[RequireRole("Participant")]
        public async Task<IActionResult> Enrol(int eventId, [FromBody] EnrolmentDto dto)
        {
            var userId = 3;// HttpContext.Session.GetInt("UserId");
            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.UserID == userId);
            if (participant == null) return NotFound("Participant profile not found.");

            var enrolment = new Enrolment
            {
                ParticipantID = participant.ParticipantID,
                EventID = eventId,
                CategoryID = dto.CategoryId,
                Status = "Pending"
            };
            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();
            return Ok("Enrolled successfully.");
        }

        // Organiser views enrolments for their event
        [HttpGet("events/{eventId}/enrolments")]
        //[RequireRole("Organiser")]
        public async Task<IActionResult> GetEventEnrolments(int eventId)
        {
            var enrolments = await _context.Enrolments
                .Include(e => e.Participant)
                .Include(e => e.Category)
                .Where(e => e.EventID == eventId).ToListAsync();
            return Ok(enrolments);
        }

        // Participant views their own enrolments
        [HttpGet("my-enrolments")]
        //[RequireRole("Participant")]
        public async Task<IActionResult> GetMyEnrolments()
        {
            var userId = 3; //HttpContext.Session.GetInt("UserId");
            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.UserID == userId);

            var enrolments = await _context.Enrolments
                .Include(e => e.Event)
                .Where(e => e.ParticipantID == participant.ParticipantID).ToListAsync();
            return Ok(enrolments);
        }
    }
}