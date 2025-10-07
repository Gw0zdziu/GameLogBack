using System.ComponentModel.DataAnnotations;

namespace GameLogBack.Dtos.Category;

public class CategoryDto : CategoryBase
{
    [Required(ErrorMessage = "Category is required")]
    public string CategoryId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
}