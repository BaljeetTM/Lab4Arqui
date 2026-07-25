using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Shortly.Domain.Entities;

[Table("users")]
[Index(nameof(Email), IsUnique = true)]
public class User
{
    [Key]
    public long Id { get; private set; }

    [Required]
    [MaxLength(320)]
    public string Email { get; private set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Password { get; private set; } = null!;

    public ICollection<Link> Links { get; private set; } = new List<Link>();

    private User()
    {
    }

    public User(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));

        Email = email.Trim().ToLowerInvariant();
        SetPassword(password);
    }

    private void SetPassword(string plainPassword)
    {
        if (string.IsNullOrWhiteSpace(plainPassword))
            throw new ArgumentException("Password is required", nameof(plainPassword));

        if (plainPassword.Length < 6)
            throw new ArgumentException("Password must be at least 6 characters.", nameof(plainPassword));

        Password = BCrypt.Net.BCrypt.HashPassword(plainPassword);
    }
}
