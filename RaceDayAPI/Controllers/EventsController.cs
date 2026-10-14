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
    public class EventsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;
        public EventsController(RaceDayDbContext context) { _context = context; }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.Events.Include(e => e.Organiser).ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var evt = await _context.Events.Include(e => e.Categories).FirstOrDefaultAsync(e => e.EventID == id);
            if (evt == null) return NotFound();
            return Ok(evt);
        }

        [HttpPost]
        //[RequireRole("Organiser")]
        public async Task<IActionResult> Create([FromBody] EventCreateDto dto)
        {
            var orgId = 1;//HttpContext.Session.GetInt("UserId");
            // Find the OrganiserID based on the logged-in User's UserID
            var organiser = await _context.Organisers.FirstOrDefaultAsync(o => o.UserID == orgId);

            if (organiser == null) return NotFound("Organiser not found."); // Safety check
            var newEvent = new Event
            {
                OrganiserID = organiser.OrganiserID,
                Name = dto.Name,
                Description = dto.Description,
                EventDate = dto.EventDate,
                Location = dto.Location,
                Distance = dto.Distance,
                EventType = dto.EventType
            };
            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = newEvent.EventID }, newEvent);
        }

        [HttpPut("{id}")]
        //[RequireRole("Organiser")]
        public async Task<IActionResult> Update(int id, [FromBody] EventCreateDto dto)
        {
            var evt = await _context.Events.FindAsync(id);
            if (evt == null) return NotFound();

            evt.Name = dto.Name; evt.Description = dto.Description;
            evt.EventDate = dto.EventDate; evt.Location = dto.Location;
            evt.Distance = dto.Distance; evt.EventType = dto.EventType;

            await _context.SaveChangesAsync();
            return Ok(evt);
        }

        [HttpDelete("{id}")]
       // [RequireRole("Organiser")]
        public async Task<IActionResult> Delete(int id)
        {
            var evt = await _context.Events.FindAsync(id);
            if (evt == null) return NotFound();
            _context.Events.Remove(evt);
            await _context.SaveChangesAsync();
            return Ok("Event deleted.");
        }
    }
}
