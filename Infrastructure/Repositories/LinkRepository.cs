using Microsoft.EntityFrameworkCore;
using Shortly.Application.Interfaces;
using Shortly.Domain.Entities;
using Shortly.Infrastructure.Persistence;

namespace Shortly.Infrastructure.Repositories;

public sealed class LinkRepository : ILinkRepository
{
    private readonly AppDbContext _context;

    public LinkRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Task<Link?> GetByIdAsync(long id)
        => _context.Links.FirstOrDefaultAsync(l => l.Id == id);

    public Task<Link?> GetByShortUrlAsync(string shortUrl)
        => _context.Links.AsNoTracking().FirstOrDefaultAsync(l => l.ShortUrl == shortUrl);

    public Task<List<Link>> GetAllAsync()
        => _context.Links.AsNoTracking().ToListAsync();

    public Task<List<Link>> GetByUserIdAsync(long userId)
        => _context.Links.AsNoTracking().Where(l => l.UserId == userId).ToListAsync();

    public async Task AddAsync(Link link)
        => await _context.Links.AddAsync(link);

    public Task SaveChangesAsync()
        => _context.SaveChangesAsync();
}
