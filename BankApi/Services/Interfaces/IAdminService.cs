using BankApi.Dto;
using BankApi.Dto.Request;
using BankApi.Dto.Response;
using BankApi.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankApi.Services.Interfaces
{
    public interface IAdminService
    {
        // Admin

        // Roles
        Task<RoleRequestDto> CreateRoleAsync(string roleName);
        Task<bool> DeleteRoleAsync(int roleId);
        Task<List<RoleResponseDto>> GetRolesAsync();

        Task<string> ApproveAccountRequest(int userId, bool isApproved,AccountType accountType, string? rejectedReason);
        Task<IEnumerable<UserStatusDto>> GetAllUsersWithStatusAsync();
        // Bank Managers
        Task<BankManagerDto> CreateBankManagerAsync(BankManagerDto bankManagerDto);
        Task<List<AdminResponseDto>> GetBankManagersAsync();

        // Users
        Task<List<UserResponseDto>> GetAllUsersExceptAdminAsync();
        Task<string> VerifyOtpAsync(string email, string otp);
        Task<BankMangerUpdateDto> UpdateUserAsync(int userId, BankMangerUpdateDto bankMangerUpdateDto);
        Task<bool> DeleteUserAsync(int userId);

        //account 
        Task<List<UserResponseDto>> GetApprovedAccountsAsync();
        Task<(bool success, string errorMessage)> ApproveTransactionAsync(int transactionId);
        Task<List<TransactionResponseDto>> GetPendingTransactionsAsync();
        Task<bool> RejectTransactionAsync(int transactionId, string reason);
    }
}
