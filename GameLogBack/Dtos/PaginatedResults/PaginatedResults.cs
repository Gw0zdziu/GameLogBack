namespace GameLogBack.Dtos.PaginatedResults;

public class PaginatedResults<T>
{
    public List<T> Results { get; set; }
    public int TotalAmount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int FirstItemIndexList { get; set; }
    public int LastItemIndexList { get; set; }
    public List<int> AmountPagesList { get; set; }
}