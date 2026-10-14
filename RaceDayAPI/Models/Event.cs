using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

//This the event class which is used to store the event information in the database. It has a one to many relationship with the category class and a many to one relationship with the organiser class. The event class has a primary key of EventID and a foreign key of OrganiserID. The event class also has a collection of categories which is used to store the categories associated with the event.
namespace RaceDayAPI.Models
{
    public class Event
    {
        
            [Key] public int EventID { get; set; }
            [Required] public int OrganiserID { get; set; }
            [ForeignKey("OrganiserID")] public Organiser Organiser { get; set; } = null!;
            [Required, MaxLength(200)] public string Name { get; set; } = string.Empty;
            [Required] public string Description { get; set; } = string.Empty;
            [Required] public DateTime EventDate { get; set; }
            [Required, MaxLength(200)] public string Location { get; set; } = string.Empty;
            [Required, Column(TypeName = "decimal(5,2)")] public decimal Distance { get; set; }
            [Required, MaxLength(150)] public string EventType { get; set; } = string.Empty;
            [MaxLength(500)] public string? BannerImageUrl { get; set; }

            public ICollection<Category> Categories { get; set; } = new List<Category>();
        

    }
}
