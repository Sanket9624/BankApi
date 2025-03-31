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
        private readonly IAuthRepository _authRepository;

        public AdminRepository(BankDb1Context context, IAuthRepository authRepository)
        {
            _context = context;
            _authRepository = authRepository;
        }

        //1.Role Related Operation

        //Create a New Role
        public async Task<RoleMaster> CreateRoleAsync(RoleMaster role)
        {
            await _context.RoleMaster.AddAsync(role);
            await _context.SaveChangesAsync();
            return role;
        }

        //Get All the Roles
        public async Task<List<RoleMaster>> GetRolesAsync() =>
            await _context.RoleMaster.AsNoTracking().ToListAsync();

        //Get RoleById
        public async Task<bool> GetRoleByIdAsync(int roleId) =>
            await _context.RoleMaster.AsNoTracking().AnyAsync(r => r.RoleId == roleId);

        //Delete existing Role
        public async Task<bool> DeleteRoleAsync(int roleId)
        {
            //prevent deletion of essential Roles
            if (roleId <= 3) return false;
            var role = await _context.RoleMaster.FindAsync(roleId);
            if (role == null) return false;

            _context.RoleMaster.Remove(role);
            await _context.SaveChangesAsync();
            return true;
        }

        //2.Manager Related Operation

        //Create a Bank Manager
        public async Task<Users> CreateBankManagerAsync(Users bankManager)
        {
            await _context.Users.AddAsync(bankManager);
            await _context.SaveChangesAsync();
            return bankManager;
        }
        //Get the list of Bank Managers
        public async Task<List<Users>> GetBankManagersAsync() =>
            await _context.Users
                .Where(u => u.RoleId == 2 && u.IsEmailVerified == true)
                .Include(u => u.RoleMaster)
                .AsNoTracking()
                .ToListAsync();

        //3.Customer Related Operation

        //Get the customerList 
        public async Task<List<Users>> GetAllUsersExceptAdminAsync() =>
            await _context.Users
                .Where(u => u.RoleId == 3 && u.IsEmailVerified == true && u.RequestStatus == RequestStatus.Approved)
                .Include(u => u.RoleMaster)
                .AsNoTracking()
                .ToListAsync();

        // Get all active users
        public async Task<IEnumerable<Users>> GetAllUsersAsync() =>
        await _context.Users
            .Include(u => u.RoleMaster)
            .ToListAsync();


        //4.User Related Operation

        //update users
        public async Task<Users> UpdateUserAsync(int userId, BankMangerUpdateDto dto)
        {
            var existingUser = await _context.Users.FindAsync(userId);
            if (existingUser == null || existingUser.RoleId == 1) return null;

            existingUser.FirstName = dto.FirstName ?? existingUser.FirstName;
            existingUser.LastName = dto.LastName ?? existingUser.LastName;
            existingUser.MobileNo = dto.MobileNo ?? existingUser.MobileNo;
            existingUser.Address = dto.Address ?? existingUser.Address;

            await _context.SaveChangesAsync();
            return existingUser;
        }

        //Get account Detail By UserId
        public async Task<Account> GetAccountByUserIdAsync(int accountId)
        {
            var account = await _context.Account
                .FirstOrDefaultAsync(a => a.AccountId == accountId);

            if (account == null)
            {
                Console.WriteLine($"No account found for  {accountId}");
            }

            return account;
        }


        //SoftDelete the User
        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.RoleId == 1) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }


        //5.Account Related Operation

        //Get Accountnumber of customers
        public async Task<List<string>> GetAllAccountNumbersAsync()
        {
            return await _context.Account
                .Select(a => a.AccountNumber)
                .ToListAsync();
        }

        //Update Account
        public async Task UpdateAccountAsync(Account account)
        {
            _context.Account.Update(account);
            await _context.SaveChangesAsync();
        }

        //CreateAccount
        public async Task CreateAccountAsync(Account account)
        {
            await _context.Account.AddAsync(account);
            await _context.SaveChangesAsync();
        }
        // Get list of approved accounts
        public async Task<List<Users>> GetApproveOrRejectedAccountsAsync() =>
            await _context.Users
                .Where(u => u.RequestStatus == RequestStatus.Approved || u.RequestStatus == RequestStatus.Rejected && u.RoleMaster.RoleName == "Customer")
                .AsNoTracking()
                .ToListAsync();

        public async Task<List<Transactions>> GetTransactionsByStatusAsync(TransactionStatus status)
        {
            return await _context.Transactions
                .Where(t => t.Status == status)
                .Include(t => t.SenderAccount.Users)
                .Include(t => t.ReceiverAccount.Users)
                .OrderByDescending(t => t.TransactionDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Transactions> GetTransactionByIdAsync(int transactionId)
        {
            return await _context.Transactions.FindAsync(transactionId);
        }
        public async Task UpdateTransactionAsync(Transactions transaction)
        {
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

    }
}
