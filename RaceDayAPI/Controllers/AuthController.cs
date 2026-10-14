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
    public class AuthController : ControllerBase
    {
        private readonly RaceDayDbContext _context;
        public AuthController(RaceDayDbContext context) { _context = context; }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return Conflict("Email already exists.");
            // Validate required fields for roles
            if (dto.Role == "Participant" && dto.DateOfBirth == null)
                return BadRequest("DateOfBirth is required for Participant registration.");

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role,
                CreatedAt = DateTime.Now,
                // Populate profile fields on the Users table too
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                OrganisationName = dto.OrganisationName,
                PhoneNumber = dto.PhoneNumber,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender,
                EmergencyContact = dto.EmergencyContact,
                ClubName = dto.ClubName
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            if (dto.Role == "Organiser")
            {
                _context.Organisers.Add(new Organiser
                {
                    UserID = user.UserID,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    OrganisationName = dto.OrganisationName,
                    PhoneNumber = dto.PhoneNumber ?? string.Empty
                });
            }
            else if (dto.Role == "Participant")
            {
                _context.Participants.Add(new Participant
                {
                    UserID = user.UserID,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    DateOfBirth = dto.DateOfBirth!.Value,
                    Gender = dto.Gender ?? string.Empty,
                    EmergencyContact = dto.EmergencyContact ?? string.Empty,
                    ClubName = dto.ClubName
                });
            }
            await _context.SaveChangesAsync();
            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials.");

            HttpContext.Session.SetInt("UserId", user.UserID);
            HttpContext.Session.SetString("UserRole", user.Role);

            return Ok(new { message = "Login successful", role = user.Role });
        }

        [HttpPost("logout")]
        [RequireAuth]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok("Logged out.");
        }
    }
}
