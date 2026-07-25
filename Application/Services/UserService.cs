using System.Collections.Concurrent;
using Shortly.Application.DTOs;
using Shortly.Application.Interfaces;
using Shortly.Domain.Entities;

namespace Shortly.Application.Services;

public sealed class UserService : IUserService
{
    private static readonly ConcurrentDictionary<string, ConcurrentQueue<DateTime>> _failedAttempts = new();

    private readonly ILogger<UserService> _logger;
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<UserResponse> Register(string email, string password)
    {
        var normalizedEmail = NormalizeEmail(email);
        _logger.LogDebug("Attempting to register user with email: {Email}", normalizedEmail);

        var existUser = await _userRepository.ExistsByEmailAsync(normalizedEmail);
        if (existUser)
        {
            _logger.LogError("Registration failed: Email {Email} is already in use.", normalizedEmail);
            throw new InvalidOperationException("Email is already registered.");
        }

        var user = new User(normalizedEmail, password);

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        _logger.LogInformation("User registered successfully with email: {Email} and id: {Id}.", user.Email, user.Id);
        return UserResponse.From(user);
    }

    public async Task<List<UserResponse>> GetAllUsers()
    {
        _logger.LogDebug("Retrieving all users from the database ..");
        var users = await _userRepository.GetAllAsync();

        _logger.LogInformation("Retrieved {Count} users from the database.", users.Count);
        return users.Select(UserResponse.From).ToList();
    }

    public async Task<UserResponse> Login(string email, string password)
    {
        var normalizedEmail = NormalizeEmail(email);
        _logger.LogDebug("Attempting login for email: {Email}", normalizedEmail);

        CheckRateLimit(normalizedEmail);

        var user = await _userRepository.GetByEmailAsync(normalizedEmail);
        if (user is null)
        {
            RecordFailedAttempt(normalizedEmail);
            _logger.LogWarning("Login failed: No user found with email {Email}.", normalizedEmail);
            throw new KeyNotFoundException($"No user found with email '{normalizedEmail}'.");
        }

        if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            RecordFailedAttempt(normalizedEmail);
            _logger.LogWarning("Login failed: Invalid password for email {Email}.", normalizedEmail);
            throw new UnauthorizedAccessException("Invalid password.");
        }

        ResetFailedAttempts(normalizedEmail);
        _logger.LogInformation("User logged in successfully with email: {Email} and id: {Id}.", user.Email, user.Id);
        return UserResponse.From(user);
    }

    public async Task<UserResponse> GetUserByEmail(string email)
    {
        var normalizedEmail = NormalizeEmail(email);
        _logger.LogDebug("Retrieving user with email: {Email}", normalizedEmail);

        var user = await _userRepository.GetByEmailAsync(normalizedEmail);
        if (user is null)
        {
            _logger.LogWarning("User not found with email {Email}.", normalizedEmail);
            throw new KeyNotFoundException($"No user found with email '{normalizedEmail}'.");
        }

        _logger.LogInformation("User retrieved successfully with email: {Email} and id: {Id}.", user.Email, user.Id);
        return UserResponse.From(user);
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    private static void CheckRateLimit(string email)
    {
        var now = DateTime.UtcNow;
        var window = _failedAttempts.GetOrAdd(email, _ => new ConcurrentQueue<DateTime>());

        while (window.TryPeek(out var t) && now - t > TimeSpan.FromMinutes(5))
            window.TryDequeue(out _);

        if (window.Count >= 10)
            throw new InvalidOperationException("Too many login attempts. Please try again later.");
    }

    private static void RecordFailedAttempt(string email)
    {
        var queue = _failedAttempts.GetOrAdd(email, _ => new ConcurrentQueue<DateTime>());
        queue.Enqueue(DateTime.UtcNow);
    }

    private static void ResetFailedAttempts(string email)
    {
        _failedAttempts.TryRemove(email, out _);
    }

}
