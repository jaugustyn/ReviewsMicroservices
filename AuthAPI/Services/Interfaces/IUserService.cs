using AuthAPI.Dto.Users;
using Core.Enumerations;

namespace AuthAPI.Services.Interfaces;

public interface IUserService
{
    Task<UserDto> GetUserByEmailAsync(string email);
    Task<UserDto> CreateAsync(UserCreateDto entity);
    Task<UserDto> UpdateAsync(Guid id, UserUpdateDto entity);
    Task<UserDto> ChangePassword(Guid id, UserChangePasswordDto entity);
    Task<UserDto> ChangeRole(Guid id, Role role);
    Task DeleteAsync(Guid id);
}