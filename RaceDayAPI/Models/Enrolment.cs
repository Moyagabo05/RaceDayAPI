using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// This is the enrollment model
namespace RaceDayAPI.Models
{
    public class Enrolment
    {
        [Key] public int EnrolmentID { get; set; }
        [Required] public int ParticipantID { get; set; }
        [ForeignKey("ParticipantID")] public Participant Participant { get; set; } = null!;
        [Required] public int EventID { get; set; }
        [ForeignKey("EventID")] public Event Event { get; set; } = null!;
        [Required] public int CategoryID { get; set; }
        [ForeignKey("CategoryID")] public Category Category { get; set; } = null!;
        [Required, MaxLength(50)] public string Status { get; set; } = "Pending";
        public DateTime EnrolmentDate { get; set; } = DateTime.Now;
    }
}
