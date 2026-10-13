using JetBrains.Annotations;
namespace GameLogBack.Dtos.GameBrainApi.Response;

public class GameSearchResponse
{
    public Sorting Sorting { get; set; }
    public List<string> ActiveFilterOptions { get; set; }
    public string Query { get; set; }
    public int TotalResults { get; set; }
    public int Limit { get; set; }
    public int Offset { get; set; }
    public List<Result> Results { get; set; }
    public List<SortingOptions> SortingOptions { get; set; }
}

public class Sorting
{
    public string Key { get; set; }
    public string Direction { get; set; }
}

public class Result
{
    public int Id { get; set; }
    public decimal? Year { get; set; }
    public string Name { get; set; }
    public string Genre { get; set; }
    public string Image { get; set; }
    public string Link { get; set; }
    public Rating Rating { get; set; }
    public bool AdultOnly { get; set; }
    public List<string> Screenshots { get; set; }
    public string MicroTrailer { get; set; }
    public string Gameplay { get; set; }
    public string ShortDescription { get; set; }
}

public class Rating
{
    public decimal? Mean { get; set; }
    public decimal? Count { get; set; }
}

public class SortingOptions
{
    [CanBeNull] public string Name { get; set; }
    [CanBeNull] public string Sort { get; set; }
    [CanBeNull] public string Key { get; set; }
}

public class GameDetails
{
    public string Name { get; set; }
    public string Image { get; set; }
}
