using JetBrains.Annotations;

namespace GameLogBack.Dtos.GameBrainApi.Response;

public class GamesBrain
{
    public Sorting sorting { get; set; }
    public List<string> active_filter_options  { get; set; }
    public string query {get; set;}
    public int total_results  { get; set; }
    public int limit  { get; set; }
    public int offset  { get; set; }
    public List<Result> results { get; set; }
    public List<SortingOptions> sorting_options { get; set; }


}

public class Sorting
{
    public object key { get; set; }
    public object direction { get; set; }
}

public class Result
{
    public int id { get; set; }
    public decimal? year { get; set; }
    public string name { get; set; }
    public string genre { get; set; }
    public string image { get; set; }
    public string link { get; set; }
    public Rating rating { get; set; }
    public bool adult_only  { get; set; }
    public List<string> screenshots  { get; set; }
    public string micro_trailer  { get; set; }
    public string gameplay { get; set; }
    public string short_description  { get; set; }
}

public class Rating
{
    public decimal? mean { get; set; }
    public decimal? count  { get; set; }
}

public class SortingOptions
{
    [CanBeNull] public string name { get; set; }
    [CanBeNull] public string sort { get; set; }
    [CanBeNull] public string key  { get; set; }
}

public class GameDetails
{
    public string name { get; set; }
    public string image { get; set; }
}
