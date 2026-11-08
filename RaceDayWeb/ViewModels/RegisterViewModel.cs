using System.ComponentModel.DataAnnotations;

namespace RaceDayWeb.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Participant";

        // Participant specific fields
        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Gender { get; set; } = "Male";

        [Required]
        public string EmergencyContact { get; set; } = string.Empty;

        public string? ClubName { get; set; }
    }
}