using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using RaceDayAPI.Helpers;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers
{
    [Route("api/events/{eventId}/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDayDbContext _context;
        public CategoriesController(RaceDayDbContext context) { _context = context; }

        [HttpGet]
        public async Task<IActionResult> GetCategories(int eventId)
        {
            var cats = await _context.Categories.Where(c => c.EventID == eventId).ToListAsync();
            return Ok(cats);
        }

        [HttpPost]
        [RequireRole("Organiser")]
        public async Task<IActionResult> AddCategory(int eventId, [FromBody] CategoryCreateDto dto)
        {
            if (!await OwnsEventAsync(eventId)) return Forbid();

            var cat = new Category
            {
                EventID = eventId,
                CategoryName = dto.CategoryName,
                MinAge = dto.MinAge,
                MaxAge = dto.MaxAge
            };
            _context.Categories.Add(cat);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCategories), new { eventId }, cat);
        }

        [HttpPut("{categoryId}")]
        [RequireRole("Organiser")]
        public async Task<IActionResult> UpdateCategory(int eventId, int categoryId, [FromBody] CategoryCreateDto dto)
        {
            if (!await OwnsEventAsync(eventId)) return Forbid();
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryID == categoryId && c.EventID == eventId);
            if (category == null) return NotFound();

            category.CategoryName = dto.CategoryName;
            category.MinAge = dto.MinAge;
            category.MaxAge = dto.MaxAge;
            await _context.SaveChangesAsync();
            return Ok(category);
        }

        [HttpDelete("{categoryId}")]
        [RequireRole("Organiser")]
        public async Task<IActionResult> DeleteCategory(int eventId, int categoryId)
        {
            if (!await OwnsEventAsync(eventId)) return Forbid();
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryID == categoryId && c.EventID == eventId);
            if (category == null) return NotFound();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private async Task<bool> OwnsEventAsync(int eventId)
        {
            var userId = HttpContext.GetUserId();
            if (userId == null) return false;

            return await _context.Events.AnyAsync(e => e.EventID == eventId &&
                e.Organiser.UserID == userId.Value);
        }
    }
}