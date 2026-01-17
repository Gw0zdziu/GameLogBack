using System.ComponentModel.DataAnnotations;


namespace GameLogBack.Dtos.Category;

public class CategoryBaseDto
{
    public string CategoryName { get; set; }
    public string Description { get; set; }
}