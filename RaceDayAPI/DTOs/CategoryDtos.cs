using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.DTOs
{
    // Used for POST (Adding a category)
    public class CategoryCreateDto
    {
        [Required, MaxLength(150)]
        public string CategoryName { get; set; } = string.Empty;

        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
    }

    // Used for GET (Returning categories)
    public class CategoryDto
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
    }
}