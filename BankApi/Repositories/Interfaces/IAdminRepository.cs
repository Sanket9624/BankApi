using BankApi.Dto.Request;
using BankApi.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankApi.Repositories.Interfaces
{
    public interface IAdminRepository
    {

        // Role Management
        Task<RoleMaster> CreateRoleAsync(RoleMaster role);
        Task<List<RoleMaster>> GetRolesAsync();
        Task<bool> GetRoleByIdAsync(int roleId);
        Task<bool> DeleteRoleAsync(int roleId);

        // Manager Management
        Task<Users> CreateBankManagerAsync(Users bankManager);
        Task<List<Users>> GetBankManagersAsync();

        // Customer Management
        Task<List<Users>> GetAllUsersExceptAdminAsync();
        Task<IEnumerable<Users>> GetAllUsersAsync();

        // User Management
        Task<Users> UpdateUserAsync(int userId, BankMangerUpdateDto dto);
        Task<Account> GetAccountByUserIdAsync(int userId);
        Task<bool> DeleteUserAsync(int userId);

        // Account Management
        Task<List<string>> GetAllAccountNumbersAsync();
        Task UpdateAccountAsync(Account account);
        Task CreateAccountAsync(Account account);

        // Account Request Tracking
        Task UpdateTransactionAsync(Transactions transaction);
        Task<Transactions> GetTransactionByIdAsync(int transactionId);
        Task<List<Users>> GetApproveOrRejectedAccountsAsync();
        Task<List<Transactions>> GetTransactionsByStatusAsync(TransactionStatus status);
    }
}
