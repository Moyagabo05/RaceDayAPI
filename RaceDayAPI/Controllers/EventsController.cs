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

        [HttpGet("dashboard")]
        [RequireRole("Organiser")]
        public async Task<IActionResult> GetDashboard()
        {
            var userId = HttpContext.GetUserId();
            if (userId == null) return Unauthorized("Not logged in.");

            var organiser = await _context.Organisers.FirstOrDefaultAsync(o => o.UserID == userId.Value);
            if (organiser == null) return NotFound("Organiser profile not found.");

            var events = _context.Events.Where(e => e.OrganiserID == organiser.OrganiserID);
            var upcomingEvents = await events
                .Where(e => e.EventDate >= DateTime.Now)
                .OrderBy(e => e.EventDate)
                .Select(e => new DashboardEventDto
                {
                    EventID = e.EventID,
                    Name = e.Name,
                    EventDate = e.EventDate,
                    EnrolmentCount = _context.Enrolments.Count(enrolment => enrolment.EventID == e.EventID)
                }).ToListAsync();

            return Ok(new DashboardDto
            {
                TotalEvents = await events.CountAsync(),
                TotalEnrolments = await _context.Enrolments.CountAsync(e => e.Event.OrganiserID == organiser.OrganiserID),
                UpcomingEvents = upcomingEvents
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // Project the response instead of serializing EF navigation properties.
            // This prevents the Event -> Category -> Event reference cycle from causing a 500.
            var evt = await _context.Events
                .Where(e => e.EventID == id)
                .Select(e => new
                {
                    e.EventID,
                    e.OrganiserID,
                    e.Name,
                    e.Description,
                    e.EventDate,
                    e.Location,
                    e.Distance,
                    e.EventType,
                    e.BannerImageUrl,
                    Categories = e.Categories.Select(c => new
                    {
                        c.CategoryID,
                        c.EventID,
                        c.CategoryName,
                        c.MinAge,
                        c.MaxAge
                    }).ToList()
                })
                .FirstOrDefaultAsync();
            if (evt == null) return NotFound();
            return Ok(evt);
        }

        [HttpPost]
        [RequireRole("Organiser")]
        public async Task<IActionResult> Create([FromBody] EventCreateDto dto)
        {
            var userId = HttpContext.GetUserId(); // Uses the new helper!
            if (userId == null) return Unauthorized("Not logged in.");

            var organiser = await _context.Organisers.FirstOrDefaultAsync(o => o.UserID == userId.Value);
            if (organiser == null) return NotFound("Organiser profile not found.");

            var newEvent = new Event
            {
                OrganiserID = organiser.OrganiserID,
                Name = dto.Name,
                Description = dto.Description,
                EventDate = dto.EventDate,
                Location = dto.Location,
                Distance = dto.Distance,
                EventType = dto.EventType,
                BannerImageUrl = dto.BannerImageUrl
            };
            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = newEvent.EventID }, newEvent);
        }

        [HttpPut("{id}")]
        [RequireRole("Organiser")]
        public async Task<IActionResult> Update(int id, [FromBody] EventCreateDto dto)
        {
            var userId = HttpContext.GetUserId();
            if (userId == null) return Unauthorized("Not logged in.");
            var organiser = await _context.Organisers.FirstOrDefaultAsync(o => o.UserID == userId.Value);
            if (organiser == null) return NotFound("Organiser profile not found.");

            var evt = await _context.Events.FindAsync(id);
            if (evt == null) return NotFound();
            if (evt.OrganiserID != organiser?.OrganiserID) return Forbid();

            evt.Name = dto.Name; evt.Description = dto.Description;
            evt.EventDate = dto.EventDate; evt.Location = dto.Location;
            evt.Distance = dto.Distance; evt.EventType = dto.EventType;
            if (!string.IsNullOrEmpty(dto.BannerImageUrl)) evt.BannerImageUrl = dto.BannerImageUrl;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Event updated successfully.", eventId = evt.EventID });
        }

        [HttpDelete("{id}")]
        [RequireRole("Organiser")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = HttpContext.GetUserId(); // Uses the new helper!
            if (userId == null) return Unauthorized("Not logged in.");

            var organiser = await _context.Organisers.FirstOrDefaultAsync(o => o.UserID == userId.Value);
            if (organiser == null) return NotFound("Organiser profile not found.");

            var evt = await _context.Events.FindAsync(id);
            if (evt == null) return NotFound();
            if (evt.OrganiserID != organiser.OrganiserID) return Forbid();

            // Relationships intentionally use Restrict deletes, so remove dependent
            // rows explicitly before deleting the event.
            var enrolmentIds = await _context.Enrolments
                .Where(e => e.EventID == id)
                .Select(e => e.EnrolmentID)
                .ToListAsync();
            var results = await _context.Results
                .Where(r => enrolmentIds.Contains(r.EnrolmentID))
                .ToListAsync();
            var enrolments = await _context.Enrolments
                .Where(e => e.EventID == id)
                .ToListAsync();
            var categories = await _context.Categories
                .Where(c => c.EventID == id)
                .ToListAsync();

            _context.Results.RemoveRange(results);
            _context.Enrolments.RemoveRange(enrolments);
            _context.Categories.RemoveRange(categories);
            try
            {
                _context.Events.Remove(evt);
                await _context.SaveChangesAsync();
                return Ok("Event deleted successfully.");
            }
            catch (DbUpdateException)
            {
                return Conflict("The event could not be deleted because related records still exist.");
            }
        }
    }
}