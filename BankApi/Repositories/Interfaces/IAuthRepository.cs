using BankApi.Entities;

namespace BankApi.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<Users> GetUserByEmailAsync(string email);
        Task<Users> GetUserByIdAsync(int userId);
        Task<Users> CreateUserAsync(Users user);

    }
}
