namespace GameLogBack.Dtos.Category;

public class CategoryByUserIdDto : CategoryBaseDto
{
    public string CategoryId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public int GamesCount { get; set; }
}