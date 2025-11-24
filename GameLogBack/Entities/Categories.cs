namespace GameLogBack.Entities;

public class Categories
{
    public string CategoryId { get; set; }
    public string CategoryName { get; set; }
    public string Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
    public string UserId { get; set; }
    public virtual Users User { get; set; }
    public virtual ICollection<Games> Games { get; set; }
    
}