using BankApi.Data;
using BankApi.Entities;
using BankApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

public class AuthRepository : BankApi.Repositories.Interfaces.IUserRepository
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
}
