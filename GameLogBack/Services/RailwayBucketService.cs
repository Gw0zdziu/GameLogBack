using Amazon.S3;
using Amazon.S3.Model;
using GameLogBack.Interfaces;
using GameLogBack.Settings;
using JetBrains.Annotations;

namespace GameLogBack.Services;

public class RailwayBucketService : IRailwayBucketService
{
    private readonly HttpClient _client;
    private readonly IAmazonS3 _s3Client;
    private readonly BucketS3 _bucketS3;

    public RailwayBucketService(HttpClient client, IAmazonS3 s3Client, IConfiguration configuration, BucketS3 bucketS3)
    {
        _client = client;
        _s3Client = s3Client;
        _bucketS3 = bucketS3;
    }

    public async Task<string> UploadFile(string userId, string fileId, string urlFile)
    {
        using var responseMessage = await _client.GetAsync(urlFile);
        responseMessage.EnsureSuccessStatusCode();
        var contentType = responseMessage.Content.Headers.ContentType?.ToString();
        var extensionFile = urlFile.Split('.')[^1];
        var pathToFileBucket = $"{userId}/{fileId}.{extensionFile}";
        await using var imageStream = await responseMessage.Content.ReadAsStreamAsync();
        var putRequest = new PutObjectRequest()
        {
            BucketName = _bucketS3.BucketName,
            Key = pathToFileBucket,
            InputStream = imageStream,
            ContentType = contentType,
            CannedACL = S3CannedACL.PublicRead,
        };
        await _s3Client.PutObjectAsync(putRequest);
        return pathToFileBucket;

    }

    public Task<string> FetchFiles(string userId, string fileId)
    {
        throw new NotImplementedException();
    }

    [CanBeNull]
    public  string FetchFile(string filePath)
    {
        if (filePath == null) return null;
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucketS3.BucketName,
            Key = filePath.TrimStart('/'),
            Expires = DateTime.UtcNow.AddHours(1),
            Verb = HttpVerb.GET
        };

        return  _s3Client.GetPreSignedURL(request);
    }
}
