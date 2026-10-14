using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


    namespace RaceDayAPI.Models
    {
        public class Organiser
        {
            [Key] public int OrganiserID { get; set; }
            [Required] public int UserID { get; set; }
            [ForeignKey("UserID")] public User User { get; set; } = null!;
            [Required, MaxLength(150)] public string FirstName { get; set; } = string.Empty;
            [Required, MaxLength(150)] public string LastName { get; set; } = string.Empty;
            [MaxLength(150)] public string? OrganisationName { get; set; }
            [Required, MaxLength(11)] public string PhoneNumber { get; set; } = string.Empty;
        }
    }
