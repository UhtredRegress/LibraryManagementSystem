using BookService.Infrastructure.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace BookService.Infrastructure;

public class MinioService : IMinioService
{
    private readonly IMinioClient _minioClient;
    private readonly IOptions<MinioSettings> _minioSettings;

    public MinioService(IOptions<MinioSettings> options)
    {
        _minioSettings = options;
        _minioClient = new MinioClient()
            .WithEndpoint(options.Value.Endpoint)
            .WithCredentials(options.Value.AccessKey, options.Value.SecretKey)
            .WithSSL(options.Value.UseSSL)
            .Build();
    }
    public async Task UploadFileAsync(IFormFile file, string fileName)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentNullException("Invalid file upload");
        }

        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentNullException("Invalid file name");
        }
        
        bool found = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_minioSettings.Value.BucketName));
        if (!found)
        {
            await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_minioSettings.Value.BucketName));
        }
        
        using var stream = file.OpenReadStream();

        await _minioClient.PutObjectAsync(new PutObjectArgs()
            .WithBucket(_minioSettings.Value.BucketName)
            .WithObject(fileName)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length)
            .WithContentType("application/pdf"));
    }

    public async Task DeleteFileAsync(string fileName)
    {
        bool found = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_minioSettings.Value.BucketName));
        if (!found)
        {
            await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_minioSettings.Value.BucketName));
        }
        
        await _minioClient.RemoveObjectAsync(new RemoveObjectArgs().WithBucket(_minioSettings.Value.BucketName).WithObject(fileName));
    }
}