namespace RaceDayAPI.Models
{
    public class Category
    {
        [System.ComponentModel.DataAnnotations.Key] public int CategoryID { get; set; }
        [System.ComponentModel.DataAnnotations.Required] public int EventID { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("EventID")] public Event Event { get; set; } = null!;
        [System.ComponentModel.DataAnnotations.Required, System.ComponentModel.DataAnnotations.MaxLength(150)] public string CategoryName { get; set; } = string.Empty;
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
    }
}
