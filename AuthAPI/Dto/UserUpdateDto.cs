namespace AuthAPI.Dto;

public class UserUpdateDto
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Username { get; init; }
    public string Email { get; init; }
    public DateTime Birthday { get; init; }
}