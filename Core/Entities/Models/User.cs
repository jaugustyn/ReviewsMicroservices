using Core.Enums;

namespace Core.Entities.Models;

public record User : IEntityBase
{
    public Guid Id { get; init; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public DateTime Birthday { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
    public Role Role { get; set; }
    public DateTimeOffset CreatedAt { get; init; }
}