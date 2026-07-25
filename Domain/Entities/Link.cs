using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Shortly.Domain.Entities;

[Table("links")]
[Index(nameof(ShortUrl), IsUnique = true)]
public class Link
{
    [Key]
    public long Id { get; private set; }

    [Required]
    [MaxLength(20248)]
    public string Url { get; private set; } = null!;

    [Required]
    [MaxLength(32)]
    public string ShortUrl { get; private set; } = null!;

    [Required] public int Clicks { get; private set; }

    [ForeignKey(nameof(User))]
    public long UserId { get; private set; }

    public User User { get; private set; } = null!;

    private Link()
    {
    }

    public Link(string url, string shortUrl, long userId)
    {
        Url = string.IsNullOrWhiteSpace(url)
            ? throw new ArgumentException("URL is required.", nameof(url))
            : url.Trim();

        ShortUrl = string.IsNullOrWhiteSpace(shortUrl)
            ? throw new ArgumentException("ShortUrl is required.", nameof(shortUrl))
            : shortUrl.Trim();

        UserId = userId > 0
            ? userId
            : throw new ArgumentOutOfRangeException(nameof(userId), "UserId must be greater than zero.");

        Clicks = 0;
    }

    public void IncrementClicks() => Clicks++;

}
