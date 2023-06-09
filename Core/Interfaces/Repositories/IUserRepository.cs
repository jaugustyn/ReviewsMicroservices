using Core.Entities.Models;

namespace Core.Interfaces.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User> GetUserByUsernameAsync(string username);
    Task<User> GetUserByEmailAsync(string email);
}