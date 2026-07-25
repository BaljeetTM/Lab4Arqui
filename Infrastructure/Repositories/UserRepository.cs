using Microsoft.EntityFrameworkCore;
using Shortly.Application.Interfaces;
using Shortly.Domain.Entities;
using Shortly.Infrastructure.Persistence;

namespace Shortly.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Task<User?> GetByEmailAsync(string email)
        => _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);

    public Task<bool> ExistsByEmailAsync(string email)
        => _context.Users.AsNoTracking().AnyAsync(u => u.Email == email);

    public Task<List<User>> GetAllAsync()
        => _context.Users.AsNoTracking().ToListAsync();

    public async Task AddAsync(User user)
        => await _context.Users.AddAsync(user);

    public Task SaveChangesAsync()
        => _context.SaveChangesAsync();
}
