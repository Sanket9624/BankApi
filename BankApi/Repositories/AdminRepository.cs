using AutoMapper;
using BankApi.Data;
using BankApi.Dto;
using BankApi.Entities;
using BankApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BankApi.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly BankDb1Context _context;
        private readonly IMapper _mapper;

        public AdminRepository(BankDb1Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<AdminDto> GetAdminAsync()
        {
            var admin = await _context.Users
                .Include(u => u.RoleMaster)
                .FirstOrDefaultAsync(u => u.RoleMaster.RoleName == "SuperAdmin");

            return _mapper.Map<AdminDto>(admin);
        }

        public async Task<RoleRequestDto> CreateRoleAsync(string roleName)
        {
            var role = new RoleMaster { RoleName = roleName };
            _context.RoleMaster.Add(role);
            await _context.SaveChangesAsync();
            return _mapper.Map<RoleRequestDto>(role);
        }

        public async Task<bool> DeleteRoleAsync(int roleId)
        {
            var role = await _context.RoleMaster.FindAsync(roleId);
            if (role == null || roleId == 1 || roleId == 2) return false;
            _context.RoleMaster.Remove(role);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<RoleResponseDto>> GetRolesAsync()
        {
            var roles = await _context.RoleMaster.ToListAsync();
            return _mapper.Map<List<RoleResponseDto>>(roles);
        }

        public async Task<BankManagerDto> CreateBankManagerAsync(BankManagerDto bankManagerDto)
        {
            // Fetch the role based on RoleName
            var role = await _context.RoleMaster.FirstOrDefaultAsync(r => r.RoleName == bankManagerDto.RoleName);

            if (role == null)
            {
                throw new Exception("Role not found.");
            }

            var bankManager = _mapper.Map<Users>(bankManagerDto);
            bankManager.PasswordHash = HashPassword(bankManagerDto.Password);
            bankManager.RoleId = role.RoleId; // Assign Role ID dynamically

            _context.Users.Add(bankManager);
            await _context.SaveChangesAsync();

            return _mapper.Map<BankManagerDto>(bankManager);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        public async Task<BankManagerDto> UpdateBankManagerAsync(int userId , BankManagerDto bankManagerDto)
        {
            var existingUser = await _context.Users.FindAsync(userId);
            if (existingUser == null || existingUser.RoleId != 2) return null;

              
            // Update user details
            _mapper.Map(bankManagerDto, existingUser);

            await _context.SaveChangesAsync();

            return _mapper.Map<BankManagerDto>(existingUser);
        }

        public async Task<List<AdminDto>> GetBankManagersAsync()
        {
            var bankManagers = await _context.Users
                .Where(u => u.RoleId == 2)
                .ToListAsync();

            return _mapper.Map<List<AdminDto>>(bankManagers);
        }
        public async Task<List<UserResponseDto>> GetAllUsersExceptAdminAsync()
        {
            var users = await _context.Users
                .Where(u => u.RoleId != 1)
                .Include(u => u.RoleMaster)
                .ToListAsync();

            return _mapper.Map<List<UserResponseDto>>(users);
        }

        public async Task<UserRequestDto> UpdateUserAsync(int userId, UserRequestDto userRequestDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(); // Begin Transaction
            try
            {
                var existingUser = await _context.Users.FindAsync(userId);
                if (existingUser == null || existingUser.RoleId == 1) return null; // Ensure Admins can't be modified


                // Update User
                _mapper.Map(userRequestDto, existingUser);
                await _context.SaveChangesAsync();

                // Update Account Type in the Accounts Table
                var existingAccount = await _context.Account.FirstOrDefaultAsync(a => a.UserId == userId);
                if (existingAccount != null)
                {
                    existingAccount.AccountType = userRequestDto.AccountType; // Update the account type
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync(); // Commit transaction if both updates succeed

                return _mapper.Map<UserRequestDto>(existingUser);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(); // Rollback if an error occurs
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.RoleId == 1) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
