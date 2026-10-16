using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.DTOs
{
    public class UpdateProfileDto
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        public string? OrganisationName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? EmergencyContact { get; set; }
        public string? ClubName { get; set; }
    }
}
