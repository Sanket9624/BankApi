using BankApi.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankApi.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        //auth service
        Task CreateUserAsync(Users user);
        Task<Users> GetUserByEmailAsync(string email);
        Task<Users> GetUserByIdAsync(int userId);
        Task UpdateUserAsync(Users user);
        Task DeleteUserAsync(Users user);

        //other validation and needed methods
        Task<RoleMaster> GetRoleByNameAsync(string roleName);
        Task SaveOtpAsync(int userId, string otp, DateTime expiry);
        Task<bool> VerifyOtpAsync(string email, string otp);
      
    }
}
