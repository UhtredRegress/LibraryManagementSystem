using Microsoft.AspNetCore.Http;

namespace BookService.Infrastructure.Interface;

public interface IMinioService
{
    Task UploadFileAsync(IFormFile file, string fileName);
    Task DeleteFileAsync(string fileName);
    Task<string> CreatePreSignedUrlAsync(string objectName);
}