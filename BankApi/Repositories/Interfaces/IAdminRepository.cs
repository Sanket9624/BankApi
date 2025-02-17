using BankApi.Dto;
using BankApi.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankApi.Repositories.Interfaces
{
    public interface IAdminRepository
    {
        Task<AdminDto> GetAdminAsync();
        Task<RoleRequestDto> CreateRoleAsync(string roleName);
        Task<bool> DeleteRoleAsync(int roleId);
        Task<List<RoleResponseDto>> GetRolesAsync();
        Task<BankManagerDto> CreateBankManagerAsync(BankManagerDto bankMangerDto);
        Task<BankManagerDto> UpdateBankManagerAsync(int userId , BankManagerDto bankManagerDto);
        Task<List<AdminDto>> GetBankManagersAsync();
        Task<List<UserResponseDto>> GetAllUsersExceptAdminAsync();
        Task<UserRequestDto> UpdateUserAsync(int userId ,UserRequestDto userRequestDto);
        Task<bool> DeleteUserAsync(int userId);
    }
}
