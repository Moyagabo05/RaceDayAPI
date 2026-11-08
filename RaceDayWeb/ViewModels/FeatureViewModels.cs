using System.ComponentModel.DataAnnotations;

namespace RaceDayWeb.ViewModels;

public class CategoryEditViewModel
{
    public int EventId { get; set; }
    public int? CategoryId { get; set; }
    [Required, StringLength(150)] public string CategoryName { get; set; } = string.Empty;
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
}

public class ProfileViewModel
{
    [Required] public string FirstName { get; set; } = string.Empty;
    [Required] public string LastName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? OrganisationName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? EmergencyContact { get; set; }
    public string? ClubName { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public IFormFile? ProfilePicture { get; set; }
}

public class EnrolmentDisplayViewModel
{
    public int EnrolmentID { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime EnrolmentDate { get; set; }
}

public class DashboardEventViewModel
{
    public int EventID { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public int EnrolmentCount { get; set; }
}

public class DashboardViewModel
{
    public int TotalEvents { get; set; }
    public int TotalEnrolments { get; set; }
    public List<DashboardEventViewModel> UpcomingEvents { get; set; } = [];
}
