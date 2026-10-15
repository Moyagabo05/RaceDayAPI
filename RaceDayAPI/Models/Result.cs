using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//This is the results model 
namespace RaceDayAPI.Models
{
    public class Result
    {
        [Key] public int ResultID { get; set; }
        [Required] public int EnrolmentID { get; set; }
        [ForeignKey("EnrolmentID")] public Enrolment Enrolment { get; set; } = null!;
        public TimeSpan? FinishTime { get; set; }
        public int? Position { get; set; }
    }
}
