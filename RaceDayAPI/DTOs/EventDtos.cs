using System;
using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.DTOs
{
    // Used for POST and PUT (Creating/Updating)
    public class EventCreateDto
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }

        [Required, MaxLength(200)]
        public string Location { get; set; } = string.Empty;

        [Required]
        public decimal Distance { get; set; }

        [Required, MaxLength(150)]
        public string EventType { get; set; } = string.Empty; // Run, Walk, Cycle

        public string? BannerImageUrl { get; set; }
    }

    // Used for GET (Returning data to the frontend/Swagger)
    public class EventDto
    {
        public int EventID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string? BannerImageUrl { get; set; }
        public string OrganiserName { get; set; } = string.Empty; // Helpful for the frontend
    }

    public class DashboardEventDto
    {
        public int EventID { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public int EnrolmentCount { get; set; }
    }

    public class DashboardDto
    {
        public int TotalEvents { get; set; }
        public int TotalEnrolments { get; set; }
        public List<DashboardEventDto> UpcomingEvents { get; set; } = new();
    }
}
