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
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;
        public ResultsController(RaceDayDbContext context) { _context = context; }

        [HttpPost("enrolments/{enrolmentId}")]
        [RequireRole("Organiser")]
        public async Task<IActionResult> AddResult(int enrolmentId, [FromBody] ResultCreateDto dto)
        {
            if (!TimeSpan.TryParse(dto.FinishTime, out var finishTime))
                return BadRequest("FinishTime must use HH:mm:ss format.");

            var result = new Result { EnrolmentID = enrolmentId, FinishTime = finishTime, Position = dto.Position };
            _context.Results.Add(result);
            await _context.SaveChangesAsync();
            return Ok("Result captured.");
        }

        [HttpGet("events/{eventId}/enrolments")]
        [RequireRole("Organiser")]
        public async Task<IActionResult> GetEventEnrolmentsForResults(int eventId)
        {
            var userId = HttpContext.GetUserId();
            if (userId == null) return Unauthorized("Not logged in.");

            var ownsEvent = await _context.Events.AnyAsync(e => e.EventID == eventId && e.Organiser.UserID == userId.Value);
            if (!ownsEvent) return Forbid();

            var enrolments = await _context.Enrolments
                .Where(e => e.EventID == eventId)
                .Select(e => new
                {
                    e.EnrolmentID,
                    ParticipantName = e.Participant.FirstName + " " + e.Participant.LastName,
                    CategoryName = e.Category.CategoryName,
                    FinishTime = _context.Results.Where(r => r.EnrolmentID == e.EnrolmentID).Select(r => r.FinishTime).FirstOrDefault(),
                    Position = _context.Results.Where(r => r.EnrolmentID == e.EnrolmentID).Select(r => r.Position).FirstOrDefault()
                }).ToListAsync();
            return Ok(enrolments);
        }

        [HttpPost("events/{eventId}/capture")]
        [RequireRole("Organiser")]
        public async Task<IActionResult> CaptureResults(int eventId, [FromBody] List<ResultCaptureDto> results)
        {
            var userId = HttpContext.GetUserId();
            if (userId == null) return Unauthorized("Not logged in.");
            if (!await _context.Events.AnyAsync(e => e.EventID == eventId && e.Organiser.UserID == userId.Value)) return Forbid();

            var eventEnrolmentIds = await _context.Enrolments
                .Where(e => e.EventID == eventId)
                .Select(e => e.EnrolmentID)
                .ToListAsync();

            foreach (var item in results.Where(r => !string.IsNullOrWhiteSpace(r.FinishTime)))
            {
                if (!eventEnrolmentIds.Contains(item.EnrolmentID))
                    return BadRequest("One or more enrolments do not belong to this event.");

                if (!TimeSpan.TryParse(item.FinishTime, out var finishTime)) return BadRequest("FinishTime must use HH:mm:ss format.");
                var existing = await _context.Results.FirstOrDefaultAsync(r => r.EnrolmentID == item.EnrolmentID);
                if (existing == null)
                    _context.Results.Add(new Result { EnrolmentID = item.EnrolmentID, FinishTime = finishTime, Position = item.Position });
                else
                {
                    existing.FinishTime = finishTime;
                    existing.Position = item.Position;
                }
            }
            await _context.SaveChangesAsync();
            return Ok("Results captured.");
        }

        [HttpGet("my-results")]
        [RequireRole("Participant")]
        public async Task<IActionResult> GetMyResults()
        {
            var userId = HttpContext.GetUserId();
            if (userId == null) return Unauthorized("Not logged in.");
            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.UserID == userId.Value);
            if (participant == null) return NotFound("Participant profile not found.");

            var results = await _context.Results.Include(r => r.Enrolment).ThenInclude(e => e.Event).Where(r => r.Enrolment.ParticipantID == participant.ParticipantID).ToListAsync();
            return Ok(results);
        }
    }
}