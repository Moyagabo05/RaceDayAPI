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
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return Conflict(new { error = "Registration failed", detail = "Email already exists." });

            if (dto.Role == "Participant" && dto.DateOfBirth == null)
                return BadRequest(new { error = "Registration failed", detail = "DateOfBirth is required for Participant." });

            if (dto.Role == "Organiser" && string.IsNullOrWhiteSpace(dto.PhoneNumber))
                return BadRequest(new { error = "Registration failed", detail = "PhoneNumber is required for Organiser." });

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Create the base User (Auth only, matching Part 1 SQL)
                var user = new User
                {
                    Email = dto.Email.Trim(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    Role = dto.Role,
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync(); // Generates UserID

                // 2. Create the Role-Specific Profile
                if (dto.Role == "Organiser")
                {
                    _context.Organisers.Add(new Organiser
                    {
                        UserID = user.UserID,
                        FirstName = dto.FirstName,
                        LastName = dto.LastName,
                        OrganisationName = dto.OrganisationName,
                        PhoneNumber = dto.PhoneNumber!
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
                        Gender = dto.Gender ?? "Not Specified",
                        EmergencyContact = dto.EmergencyContact ?? "None",
                        ClubName = dto.ClubName
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok(new { message = "User registered successfully", userId = user.UserID, role = user.Role });
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { error = "Database error", detail = dbEx.InnerException?.Message ?? dbEx.Message });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { error = "Unexpected error", detail = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized(new { error = "Login failed", detail = "Invalid credentials." });

            HttpContext.Session.SetInt("UserId", user.UserID);
            HttpContext.Session.SetString("UserRole", user.Role);

            return Ok(new { message = "Login successful", role = user.Role, userId = user.UserID });
        }

        [HttpPost("logout")]
        [RequireAuth]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok(new { message = "Logged out successfully." });
        }
    }
}