using BankApi.Dto;
using BankApi.Repositories.Interfaces;
using BankApi.Services.Interfaces;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BankApi.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<AdminDto> GetAdminAsync()
        {
            return await _adminRepository.GetAdminAsync();
        }

        public async Task<RoleRequestDto> CreateRoleAsync(string roleName)
        {
            return await _adminRepository.CreateRoleAsync(roleName);
        }

        public async Task<bool> DeleteRoleAsync(int roleId)
        {
            return await _adminRepository.DeleteRoleAsync(roleId);
        }

        public async Task<List<RoleResponseDto>> GetRolesAsync()
        {
            return await _adminRepository.GetRolesAsync();
        }

        public async Task<BankManagerDto> CreateBankManagerAsync(BankManagerDto bankManagerDto)
        {

            return await _adminRepository.CreateBankManagerAsync(bankManagerDto);
        }

        public async Task<BankManagerDto> UpdateBankManagerAsync(int userId , BankManagerDto bankManagerDto)
        {
            return await _adminRepository.UpdateBankManagerAsync(userId , bankManagerDto);
        }

        public async Task<List<AdminDto>> GetBankManagersAsync()
        {
            return await _adminRepository.GetBankManagersAsync();
        }   
        public async Task<List<UserResponseDto>> GetAllUsersExceptAdminAsync()
        {
            return await _adminRepository.GetAllUsersExceptAdminAsync();
        }

        public async Task<UserRequestDto> UpdateUserAsync(int userId , UserRequestDto userRequestDto)
        {
            //var userEntity = new BankApi.Entities.Users();
          return await  _adminRepository.UpdateUserAsync(userId , userRequestDto);
        }


        public async Task<bool> DeleteUserAsync(int userId)
        {
            return await _adminRepository.DeleteUserAsync(userId);
        }
    }
}
