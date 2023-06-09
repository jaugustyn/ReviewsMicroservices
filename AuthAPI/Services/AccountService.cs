using AuthAPI.Dto;
using AuthAPI.Dto.Auth;
using AuthAPI.Dto.Users;
using AuthAPI.Services.Interfaces;
using Core.Entities.Models;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;

namespace AuthAPI.Services;

public class AccountService : IAccountService
{
    private readonly IJwtService _jwtAuthService;
    private readonly IPasswordService _passwordService;
    private readonly ITokenRepository _tokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserService _userService;

    public AccountService(IJwtService jwtService, IUserRepository userRepository, IPasswordService passwordService,
        IUserService userService, ITokenRepository tokenRepository)
    {
        _jwtAuthService = jwtService;
        _userRepository = userRepository;
        _passwordService = passwordService;
        _userService = userService;
        _tokenRepository = tokenRepository;
    }

    public async Task<AuthResponse> Login(UserLoginDto userLoginDto)
    {
        var user = await _userRepository.GetUserByEmailAsync(userLoginDto.Email);
        var authModel = new AuthResponse {IsAuthenticated = false};

        if (user == null)
        {
            authModel.Message = $"Account with {userLoginDto.Email} doesn't exist.";
            return authModel;
        }

        if (!Verify(userLoginDto.Password, user))
        {
            authModel.Message = "Incorrect credentials.";
            return authModel;
        }

        var token = _jwtAuthService.GenerateAccessToken(user);

        authModel.IsAuthenticated = true;
        authModel.Message = "Logged in.";
        authModel.Token = token;
        authModel.Email = user.Email;
        authModel.Role = user.Role;

        var anyActiveToken = await _tokenRepository.GetByUserIdAsync(user.Id);
        if (anyActiveToken is not null && anyActiveToken.Expires >= DateTimeOffset.Now.AddHours(1))
        {
            authModel.RefreshToken = anyActiveToken.Token;
            authModel.RefreshTokenExpiration = anyActiveToken.Expires;
        }
        else
        {
            var refreshToken = _jwtAuthService.GenerateRefreshToken(user);
            authModel.RefreshToken = refreshToken.Token;
            authModel.RefreshTokenExpiration = refreshToken.Expires;

            _ = await _tokenRepository.AddTokenAsync(refreshToken);
        }

        return authModel;
    }

    public async Task<UserDto?> Register(UserCreateDto userCreateDto)
    {
        var emailCheck = await _userRepository.GetUserByEmailAsync(userCreateDto.Email);

        if (emailCheck is not null) return null;

        var result = await _userService.CreateAsync(userCreateDto);

        return result;
    }

    public async Task<UserDto?> ChangePassword(Guid id, UserChangePasswordDto userChangePasswordDto)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null) return null;

        if (!Verify(userChangePasswordDto.OldPassword, user)) return null;

        var result = await _userService.ChangePassword(id, userChangePasswordDto);

        return result;
    }

    public async Task<AuthResponse> RefreshToken(TokenRequestModel tokenRequestModel)
    {
        var authModel = new AuthResponse {IsAuthenticated = false};

        var refreshToken = await _tokenRepository.GetByTokenAsync(tokenRequestModel.RefreshToken);

        if (refreshToken is null)
        {
            authModel.Message = "The specified token does not exist.";
            return authModel;
        }

        var user = await _userRepository.GetByIdAsync(refreshToken.UserId);

        if (user is null)
        {
            authModel.Message = "Invalid client request.";
            return authModel;
        }

        if (!refreshToken.IsActive)
        {
            authModel.Message = "Token expired.";
            return authModel;
        }

        //Revoke current refresh token
        refreshToken.RevokedAt = DateTime.UtcNow;
        await _tokenRepository.UpdateTokenAsync(refreshToken);
        var newRefreshToken = _jwtAuthService.GenerateRefreshToken(user);

        var newToken = await _tokenRepository.AddTokenAsync(newRefreshToken);

        // Generate new jwt
        authModel.IsAuthenticated = true;
        authModel.Token = _jwtAuthService.GenerateAccessToken(user);
        authModel.Email = user.Email;
        authModel.Role = user.Role;
        authModel.RefreshToken = newRefreshToken.Token;
        authModel.RefreshTokenExpiration = newRefreshToken.Expires;

        return authModel;
    }

    public async Task<bool> RevokeToken(TokenRequestModel tokenRequestModel)
    {
        var refreshToken = await _tokenRepository.GetByTokenAsync(tokenRequestModel.RefreshToken);

        if (refreshToken is null) return false;
        if (!refreshToken.IsActive) return false;

        refreshToken.RevokedAt = DateTime.UtcNow;
        await _tokenRepository.UpdateTokenAsync(refreshToken);

        return true;
    }

    private bool Verify(string password, User user)
    {
        return _passwordService.VerifyPassword(password, user.PasswordHash, Convert.FromHexString(user.PasswordSalt));
    }
}