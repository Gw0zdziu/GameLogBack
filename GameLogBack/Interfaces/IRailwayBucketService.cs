namespace GameLogBack.Interfaces;

public interface IRailwayBucketService
{
    public Task<string> UploadFile(string directoryName, string fileName, string urlFile);

    public string FetchFile(string filePath);
}
