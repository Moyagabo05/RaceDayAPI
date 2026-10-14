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
        //[RequireRole("Organiser")]
        public async Task<IActionResult> AddResult(int enrolmentId, [FromBody] ResultDto dto)
        {
            var result = new Result
            {
                EnrolmentID = enrolmentId,
                FinishTime = TimeSpan.Parse(dto.FinishTime),
                Position = dto.Position
            };
            _context.Results.Add(result);
            await _context.SaveChangesAsync();
            return Ok("Result captured.");
        }

        [HttpGet("my-results")]
        //[RequireRole("Participant")]
        public async Task<IActionResult> GetMyResults()
        {
            var userId = 3;// HttpContext.Session.GetInt("UserId");
            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.UserID == userId);

            var results = await _context.Results
                .Include(r => r.Enrolment).ThenInclude(e => e.Event)
                .Where(r => r.Enrolment.ParticipantID == participant.ParticipantID).ToListAsync();
            return Ok(results);
        }
    }
}