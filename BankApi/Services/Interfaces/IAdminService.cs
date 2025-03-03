using BankApi.Dto;
using BankApi.Dto.Request;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankApi.Services.Interfaces
{
    public interface IAdminService
    {
        // Admin
        Task<AdminResponseDto> GetAdminAsync();

        // Roles
        Task<RoleRequestDto> CreateRoleAsync(string roleName);
        Task<bool> DeleteRoleAsync(int roleId);
        Task<List<RoleResponseDto>> GetRolesAsync();

        // Bank Managers
        Task<BankManagerDto> CreateBankManagerAsync(BankManagerDto bankManagerDto);
        Task<List<AdminResponseDto>> GetBankManagersAsync();

        // Users
        Task<List<UserResponseDto>> GetAllUsersExceptAdminAsync();
        Task<BankMangerUpdateDto> UpdateUserAsync(int userId, BankMangerUpdateDto bankMangerUpdateDto);
        Task<bool> DeleteUserAsync(int userId);
    }
}
