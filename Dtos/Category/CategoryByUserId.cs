namespace GameLogBack.Dtos.Category;

public class CategoryByUserId : CategoryBaseDto
{
    public string CategoryId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}