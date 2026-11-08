using System.ComponentModel.DataAnnotations;

namespace RaceDayWeb.ViewModels
{
    // Used to display events in a list
    public class EventDisplayViewModel
    {
        public int EventID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public string? BannerImageUrl { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    // Used for the Create Event form
    public class EventCreateViewModel
    {
        [Required] public string Name { get; set; } = string.Empty;
        [Required] public string Description { get; set; } = string.Empty;
        [Required] public DateTime EventDate { get; set; }
        [Required] public string Location { get; set; } = string.Empty;
        [Required] public decimal Distance { get; set; }
        [Required] public string EventType { get; set; } = "Run";

        // This is for the Azure Upload
        public IFormFile? BannerImage { get; set; }
        public string? BannerImageUrl { get; set; }
    }
   
        // Used to display full event details and categories 
        public class EventDetailsViewModel
        {
            public int EventID { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public DateTime EventDate { get; set; }
            public string Location { get; set; } = string.Empty;
            public decimal Distance { get; set; }
            public string EventType { get; set; } = string.Empty;
            public string? BannerImageUrl { get; set; }
            public List<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
        }

        //  Used for the categories inside the event details 
        public class CategoryViewModel
        {
            public int CategoryID { get; set; }
            public string CategoryName { get; set; } = string.Empty;
            public int? MinAge { get; set; }
            public int? MaxAge { get; set; }
        }
    // For the Participant's "My Enrolments" page
    public class MyEnrolmentViewModel
    {
        public int EnrolmentID { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? BannerImageUrl { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    // For the Participant's "My Results" page
    public class MyResultViewModel
    {
        public int ResultID { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public TimeSpan? FinishTime { get; set; }
        public int? Position { get; set; }
    }

    // For the Organiser to capture results
    public class CaptureResultViewModel
    {
        public int EnrolmentID { get; set; }
        public string ParticipantName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public TimeSpan? FinishTime { get; set; }
        public int? Position { get; set; }
    }

}