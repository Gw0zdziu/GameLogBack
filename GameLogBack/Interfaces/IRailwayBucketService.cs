namespace GameLogBack.Interfaces;

public interface IRailwayBucketService
{
    public Task<string> UploadFile(string userId, string fileNameInBucket, string urlFile);
}
