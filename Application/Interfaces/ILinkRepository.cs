using Shortly.Domain.Entities;

namespace Shortly.Application.Interfaces;

public interface ILinkRepository
{
    Task<Link?> GetByIdAsync(long id);
    Task<Link?> GetByShortUrlAsync(string shortUrl);
    Task<List<Link>> GetAllAsync();
    Task<List<Link>> GetByUserIdAsync(long userId);
    Task AddAsync(Link link);
    Task SaveChangesAsync();
}
