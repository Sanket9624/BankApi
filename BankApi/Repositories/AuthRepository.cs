using BankApi.Data;
using BankApi.Entities;
using BankApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

public class AuthRepository : BankApi.Repositories.Interfaces.IAuthRepository
{
    private readonly BankDb1Context _context;

    public AuthRepository(BankDb1Context context)
    {
        _context = context;
    }

    public async Task<Users> GetUserByEmailAsync(string email)
    {
        return await _context.Users.Include(u => u.Account).FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<Users> GetUserByIdAsync(int userId)
    {
        return await _context.Users
            .Include(u => u.Account) // Include account details
            .Include(u => u.RoleMaster)    // Include role details
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<Users> CreateUserAsync(Users user)
    {
        _context.Users.Add(user);   
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateUserAsync(Users user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task SaveOtpAsync(int userId, string otp, DateTime? expiry)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.Otp = otp;
            user.OtpExpiry = expiry;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Users> VerifyOtpAsync(string email, string otp)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user != null && user.Otp == otp && user.OtpExpiry > DateTime.UtcNow)
        {
            user.IsEmailVerified = true;
            user.Otp = null;
            user.OtpExpiry = null;
            await _context.SaveChangesAsync();
            return user;
        }
        return null;
    }
    public async Task CreateAccountAsync(Account account)
    {
        _context.Account.Add(account);
        await _context.SaveChangesAsync();
    }
}
