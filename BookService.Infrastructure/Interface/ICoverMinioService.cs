using Microsoft.AspNetCore.Http;

namespace BookService.Infrastructure.Interface;

public interface ICoverMinioService
{
    Task<string> UploadBookCoverAsync(int id, IFormFile file);
}