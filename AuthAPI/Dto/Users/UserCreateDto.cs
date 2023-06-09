namespace AuthAPI.Dto.Users;

public class UserCreateDto
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Username { get; init; }
    public string Email { get; init; }
    public DateTime Birthday { get; init; }
    public string Password { get; set; }
}