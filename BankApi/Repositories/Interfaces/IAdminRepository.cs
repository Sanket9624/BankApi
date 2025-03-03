using BankApi.Dto.Request;
using BankApi.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankApi.Repositories.Interfaces
{
    public interface IAdminRepository
    {
        // Admin
        Task<Users> GetAdminAsync();

        // Roles
        Task<RoleMaster> CreateRoleAsync(RoleMaster role);
        Task<bool> DeleteRoleAsync(int roleId);
        Task<bool> GetRoleByIdAsync(int roleId);
        Task<List<RoleMaster>> GetRolesAsync();

        // Bank Managers
        Task<Users> CreateBankManagerAsync(Users bankManager);
        Task<List<Users>> GetBankManagersAsync();

        // Users
        Task<List<Users>> GetAllUsersExceptAdminAsync();
        Task<Users> UpdateUserAsync(int UserId ,BankMangerUpdateDto user);
        Task<bool> DeleteUserAsync(int userId);

        // Account (if related to user updates)
        Task<Account> GetAccountByUserIdAsync(int userId);
        Task UpdateAccountAsync(Account account);
    }
}
