using Core.Entities.Models;
using Core.Enumerations;

namespace AuthAPI.Dto.Users;

public class UserDto
{
    public Guid Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Username { get; set; }
    public string Email { get; init; }
    public DateTime Birthday { get; set; }
    public Role Role { get; set; }
    public DateTimeOffset CreatedDate { get; init; }

    public static UserDto UserToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Birthday = user.Birthday,
            CreatedDate = user.CreatedAt
        };
    }
}