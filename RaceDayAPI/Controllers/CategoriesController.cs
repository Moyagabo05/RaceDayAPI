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
    }
}
