using BankApi.Data;
using BankApi.Dto.Request;
using BankApi.Entities;
using BankApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankApi.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly BankDb1Context _context;

        public AdminRepository(BankDb1Context context)
        {
            _context = context;
        }

        public async Task<Users> GetAdminAsync()
        {
            return await _context.Users
                .Include(u => u.RoleMaster)
                .FirstOrDefaultAsync(u => u.RoleMaster.RoleName == "SuperAdmin");
        }

        public async Task<RoleMaster> CreateRoleAsync(RoleMaster role)
        {
            _context.RoleMaster.Add(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task<bool> DeleteRoleAsync(int roleId)
        {
            var role = await _context.RoleMaster.FindAsync(roleId);
            if (role == null || roleId <= 2) return false; // Prevent deletion of essential roles.

            _context.RoleMaster.Remove(role);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<RoleMaster>> GetRolesAsync()
        {
            return await _context.RoleMaster.ToListAsync();
        }

        public async Task<Users> CreateBankManagerAsync(Users bankManager)
        {
            _context.Users.AddAsync(bankManager);
            await _context.SaveChangesAsync();
            return bankManager;
        }


        public async Task<List<Users>> GetBankManagersAsync()
        {
            return await _context.Users
                .Where(u => u.RoleId == 2)
                .Include(u => u.RoleMaster)
                .ToListAsync();
        }

        public async Task<List<Users>> GetAllUsersExceptAdminAsync()
        {


            return await _context.Users
                .Where(u => u.RoleId == 3)
                .Include(u => u.RoleMaster)
                .ToListAsync();
        }

        public async Task<Users> UpdateUserAsync(int userId, BankMangerUpdateDto dto)
        {
            var existingUser = await _context.Users.FindAsync(userId);
            if (existingUser == null || existingUser.RoleId == 1) return null;

            // Update only specified fields
            existingUser.FirstName = dto.FirstName ?? existingUser.FirstName;
            existingUser.LastName = dto.LastName ?? existingUser.LastName;
            existingUser.MobileNo = dto.MobileNo ?? existingUser.MobileNo;
            existingUser.Address = dto.Address ?? existingUser.Address;

            await _context.SaveChangesAsync();
            return existingUser;
        }

        public async Task<Account> GetAccountByUserIdAsync(int userId)
        {
            return await _context.Account.FirstOrDefaultAsync(a => a.UserId == userId);
        }
        public async Task<bool> GetRoleByIdAsync(int roleId)
        {
            return await _context.RoleMaster.AsNoTracking()
                .AnyAsync(r => r.RoleId == roleId);
        }

        public async Task UpdateAccountAsync(Account account)
        {
            _context.Account.Update(account);
            await _context.SaveChangesAsync();
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
