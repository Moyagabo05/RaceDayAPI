using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//This is the user model 
namespace RaceDayAPI.Models
{
    public class User
    {
        [Key] public int UserID { get; set; }
        [Required, MaxLength(255)] public string Email { get; set; } = string.Empty;
        [Required, MaxLength(255)] public string PasswordHash { get; set; } = string.Empty;
        [Required, MaxLength(50)] public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Profile fields copied from RegisterDto so they are persisted on the Users table
        [MaxLength(150)] public string? FirstName { get; set; }
        [MaxLength(150)] public string? LastName { get; set; }
        [MaxLength(150)] public string? OrganisationName { get; set; }
        [MaxLength(11)] public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [MaxLength(10)] public string? Gender { get; set; }
        [MaxLength(200)] public string? EmergencyContact { get; set; }
        [MaxLength(200)] public string? ClubName { get; set; }
    }
}
