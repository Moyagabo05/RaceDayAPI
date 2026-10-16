using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.DTOs
{
    // Used for POST (When an organiser captures a result)
    public class ResultCreateDto
    {
        [Required]
        public string FinishTime { get; set; } = string.Empty; // e.g., "01:30:00"

        [Required]
        public int Position { get; set; }
    }

    // Used for GET (Viewing results)
    public class ResultDto
    {
        public int ResultID { get; set; }
        public int EnrolmentID { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string FinishTime { get; set; } = string.Empty;
        public int? Position { get; set; }
    }
}
