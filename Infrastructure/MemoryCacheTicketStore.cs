using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Distributed;

namespace Shortly.Infrastructure;

public sealed class MemoryCacheTicketStore : ITicketStore
{
    private readonly IDistributedCache _cache;
    private const string KeyPrefix = "AuthTicket_";

    public MemoryCacheTicketStore(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        var key = KeyPrefix + Guid.NewGuid().ToString("N");
        await RenewAsync(key, ticket);
        return key;
    }

    public Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        var bytes = TicketSerializer.Default.Serialize(ticket);
        return _cache.SetAsync(key, bytes, new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(20)
        });
    }

    public async Task<AuthenticationTicket?> RetrieveAsync(string key)
    {
        var bytes = await _cache.GetAsync(key);
        if (bytes is null)
            return null;

        return TicketSerializer.Default.Deserialize(bytes);
    }

    public Task RemoveAsync(string key)
        => _cache.RemoveAsync(key);
}
