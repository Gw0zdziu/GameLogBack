using System.ComponentModel.DataAnnotations;


namespace GameLogBack.Dtos.Category;

public class CategoryBase
{
    [Required(ErrorMessage = "Name is required")]  
    [MinLength(10, ErrorMessage = "Name must be at least 10 characters long")]
    public string CategoryName { get; set; }
    public string Description { get; set; }
}