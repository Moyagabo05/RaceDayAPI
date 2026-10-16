using System;
using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.DTOs
{
    // Used for POST (When a participant enrolls)
    public class EnrolmentRequestDto
    {
        [Required]
        public int CategoryId { get; set; }
    }

    // Used for GET (Viewing enrolments)
    public class EnrolmentDto
    {
        public int EnrolmentID { get; set; }
        public int ParticipantID { get; set; }
        public string ParticipantName { get; set; } = string.Empty; // Added for easier frontend display
        public int EventID { get; set; }
        public string EventName { get; set; } = string.Empty;
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime EnrolmentDate { get; set; }
    }
}