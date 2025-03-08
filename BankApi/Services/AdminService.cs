using AutoMapper;
using BankApi.Dto;
using BankApi.Dto.Request;
using BankApi.Entities;
using BankApi.Repositories.Interfaces;
using BankApi.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BankApi.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IAuthRepository _authRepository;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        public AdminService(IAdminRepository adminRepository, IMapper mapper , IAuthService authService , IAuthRepository authRepository)
        {
            _adminRepository = adminRepository;
            _authRepository = authRepository;
            _mapper = mapper;
            _authService = authService;
        }

        public async Task<AdminResponseDto> GetAdminAsync()
        {
            var admin = await _adminRepository.GetAdminAsync();
            return _mapper.Map<AdminResponseDto>(admin);
        }

        public async Task<RoleRequestDto> CreateRoleAsync(string roleName)
        {
            var role = new RoleMaster { RoleName = roleName };
            var createdRole = await _adminRepository.CreateRoleAsync(role);
            return _mapper.Map<RoleRequestDto>(createdRole);
        }

        public async Task<bool> DeleteRoleAsync(int roleId)
        {
            return await _adminRepository.DeleteRoleAsync(roleId);
        }

        public async Task<List<RoleResponseDto>> GetRolesAsync()
        {
            var roles = await _adminRepository.GetRolesAsync();
            return _mapper.Map<List<RoleResponseDto>>(roles);
        }
        public async Task<BankManagerDto> CreateBankManagerAsync(BankManagerDto bankManagerDto)
        {
            var existingRole = await _adminRepository.GetRoleByIdAsync(bankManagerDto.RoleId);
            if (existingRole == null || bankManagerDto.RoleId == 1 || bankManagerDto.RoleId == 3)
            {
                throw new Exception($"The role with ID '{bankManagerDto.RoleId}' does not exist.");
            }
            var existingUser = await _authRepository.GetUserByEmailAsync(bankManagerDto.Email);
            if (existingUser != null)
            {
                throw new Exception("User already exists");
            }

            var bankManager = _mapper.Map<Users>(bankManagerDto);
            bankManager.PasswordHash = HashPassword(bankManagerDto.Password);
            bankManager.IsEmailVerified = false;

            var createdBankManager = await _adminRepository.CreateBankManagerAsync(bankManager);

            // Use AuthService to Send OTP
            await _authService.SendOtpAsync(createdBankManager.Email, createdBankManager.UserId);

            return _mapper.Map<BankManagerDto>(createdBankManager);
        }
        public async Task<string> VerifyOtpAsync(string email, string otp)
        {
            var user = await _authRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            if (user.Otp != otp || user.OtpExpiry < DateTime.UtcNow)
            {
                throw new Exception("Invalid or expired OTP");
            }

            user.IsEmailVerified = true;
            user.Otp = null;
            user.OtpExpiry = null;
            await _authRepository.UpdateUserAsync(user);

            return ("Manager Created Successfully");
        }

        public async Task<List<AdminResponseDto>> GetBankManagersAsync()
        {
            var bankManagers = await _adminRepository.GetBankManagersAsync();
            return _mapper.Map<List<AdminResponseDto>>(bankManagers);
        }

        public async Task<List<UserResponseDto>> GetAllUsersExceptAdminAsync()
        {
            var users = await _adminRepository.GetAllUsersExceptAdminAsync();
            return _mapper.Map<List<UserResponseDto>>(users);
        }

        public async Task<BankMangerUpdateDto> UpdateUserAsync(int userId, BankMangerUpdateDto dto)
        {
            var updatedUser = await _adminRepository.UpdateUserAsync(userId, dto);
            if (updatedUser == null) throw new Exception("User not found.");

            return _mapper.Map<BankMangerUpdateDto>(updatedUser);
        }



        public async Task<bool> DeleteUserAsync(int userId)
        {   
            return await _adminRepository.DeleteUserAsync(userId);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
