using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//This is the participants model
namespace RaceDayAPI.Models
{
    public class Participant
    {
        [Key] public int ParticipantID { get; set; }
        [Required] public int UserID { get; set; }
        [ForeignKey("UserID")] public User User { get; set; } = null!;
        [Required, MaxLength(150)] public string FirstName { get; set; } = string.Empty;
        [Required, MaxLength(150)] public string LastName { get; set; } = string.Empty;
        [Required] public DateTime DateOfBirth { get; set; }
        [Required, MaxLength(10)] public string Gender { get; set; } = string.Empty;
        [Required, MaxLength(200)] public string EmergencyContact { get; set; } = string.Empty;
        [MaxLength(200)] public string? ClubName { get; set; }
    }
}
