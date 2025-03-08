using BankApi.Entities;
using System.Threading.Tasks;
using System;

namespace BankApi.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<Users> GetUserByEmailAsync(string email);
        Task<Users> GetUserByIdAsync(int userId);
        Task<Users> CreateUserAsync(Users user);
        Task SaveOtpAsync(int userId, string otp, DateTime? expiry);
        Task<Users> VerifyOtpAsync(string email, string otp);
        Task UpdateUserAsync(Users user);
        Task CreateAccountAsync(Account account);
    }
}
