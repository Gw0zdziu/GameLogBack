namespace GameLogBack.Interfaces;

public interface IRailwayBucketService
{
    public Task<string> UploadFile(string userId, string fileId, string urlFile);

    public string FetchFile(string filePath);
}
