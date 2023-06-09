using AuthAPI.Dto;

namespace AuthAPI.Services;

public interface IAccountService
{
    Task<AuthResponse> Login(UserLoginDto userLoginDto);
    Task<UserDto?> Register(UserCreateDto userCreateDto);
    Task<UserDto?> ChangePassword(Guid id, UserChangePasswordDto userChangePasswordDto);
    Task<AuthResponse> RefreshToken(TokenRequestModel tokenRequestModel);
    Task<bool> RevokeToken(TokenRequestModel tokenRequestModel);
}