using BookService.Infrastructure.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Minio;
using Minio.DataModel.Args;

namespace BookService.Infrastructure;

public class CoverMinioService : ICoverMinioService
{
    private readonly IMinioClient _minioClient;
    private readonly string _coverBucketName = "cover";
    
    public CoverMinioService(IMinioClient minioClient)
    {
        _minioClient = minioClient;
    }
    public async Task<string> UploadBookCoverAsync(int id, IFormFile file)
    {
        string objectName = id + "_cover_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        // Ensure bucket exists
        bool found = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_coverBucketName));
        if (!found)
        {
            await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_coverBucketName));
        }
        
        using var stream = file.OpenReadStream();

        await _minioClient.PutObjectAsync(
            new PutObjectArgs()
                .WithBucket(_coverBucketName)
                .WithObject(objectName)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType(file.ContentType)
        );

        return objectName;
    }
    
    public async Task DeleteBookCoverAsync(string objectName)
    {
        if (string.IsNullOrWhiteSpace(objectName))
            throw new ArgumentException("Object name cannot be null or empty");

        // Ensure bucket exists
        bool found = await _minioClient
            .BucketExistsAsync(new BucketExistsArgs().WithBucket(_coverBucketName));

        if (!found)
        {
            throw new InvalidOperationException($"Bucket '{_coverBucketName}' does not exist.");
        }

        await _minioClient.RemoveObjectAsync(
            new RemoveObjectArgs()
                .WithBucket(_coverBucketName)
                .WithObject(objectName)
        );
    }

}