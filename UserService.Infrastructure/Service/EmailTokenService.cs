using Microsoft.Extensions.Caching.Distributed;
using UserService.Infrastructure.Service.Interface;

namespace UserService.Infrastructure.Service;

public class EmailTokenService : IEmailTokenService
{
    private readonly IDistributedCache _cache;

    public EmailTokenService(IDistributedCache cache)
    {
        _cache = cache;
    }
    public async Task<string> GenerateToken(string email, int expireSeconds)
    {
        var cacheKey = $"email:{email}"; 
        var foundKey =  await _cache.GetStringAsync(cacheKey);
        if (foundKey != null)
        {
            throw new Exception("Email token already exists"); 
        }
        
        Random random = new Random();
        string token = random.Next(0, 1000000).ToString();
        
        await _cache.SetStringAsync(cacheKey, token, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expireSeconds)
        });

        return token;
    }
    

    public async Task<bool> ValidateToken(string email, string token)
    {
        var foundValue = await  _cache.GetStringAsync($"email:{email}");
        if (foundValue != null)
        {
            return foundValue == token;
        }

        return false;
    }
}