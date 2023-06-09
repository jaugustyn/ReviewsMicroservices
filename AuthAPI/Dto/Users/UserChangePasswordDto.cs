namespace AuthAPI.Dto.Users;

public class UserChangePasswordDto
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
}