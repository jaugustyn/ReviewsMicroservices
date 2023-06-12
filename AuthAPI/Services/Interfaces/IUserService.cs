using AuthAPI.Dto.Users;
using Core.Enumerations;

namespace AuthAPI.Services.Interfaces;

public interface IUserService
{
    Task<UserDto> GetUserByEmailAsync(string email);
    Task<UserDto> CreateAsync(UserCreateDto entity);
    Task<UserDto> UpdateAsync(Guid userId, UserUpdateDto entity);
    Task<UserDto> ChangePassword(Guid userId, UserChangePasswordDto entity);
    Task<UserDto> ChangeRole(Guid userId, Role role);
    Task DeleteAsync(Guid userId);
}