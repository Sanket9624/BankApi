using BankApi.Data;
using BankApi.Entities;
using BankApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

public class AuthRepository : IAuthRepository
{
    private readonly BankDb1Context _context;

    public AuthRepository(BankDb1Context context)
    {
        _context = context;
    }

    // Create a new user (Pending approval)
    public async Task CreateUserAsync(Users user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    // Get user by email
    public async Task<Users> GetUserByEmailAsync(string email) =>
        await _context.Users
            .Include(u => u.Account)
            .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);

    // Get user by ID
    public async Task<Users> GetUserByIdAsync(int userId) =>
        await _context.Users
            .Include(u => u.Account)
            .Include(u => u.RoleMaster)
            .FirstOrDefaultAsync(u => u.UserId == userId && !u.IsDeleted);

    // Update user details
    public async Task UpdateUserAsync(Users user)
    {
        if (user != null)
        {
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
    //Delete User
    public async Task DeleteUserAsync(Users user)
    {
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
    // Get role details by name
    public async Task<RoleMaster> GetRoleByNameAsync(string roleName) =>
        await _context.RoleMaster.FirstOrDefaultAsync(r => r.RoleName == roleName);

    // Save OTP for email verification
    public async Task SaveOtpAsync(int userId, string otp, DateTime expiry)
    {
        var existingOtp = await _context.OtpVerifications.FirstOrDefaultAsync(o => o.UserId == userId);
        if (existingOtp != null)
        {
            existingOtp.Otp = otp;
            existingOtp.OtpExpiry = expiry;
        }
        else
        {
            await _context.OtpVerifications.AddAsync(new OtpVerifications
            {
                UserId = userId,
                Otp = otp,
                OtpExpiry = expiry
            });
        }
        await _context.SaveChangesAsync();
    }

    // Verify OTP and mark email as verified
    public async Task<bool> VerifyOtpAsync(string email, string otp)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        if (user == null) return false;

        var otpRecord = await _context.OtpVerifications.FirstOrDefaultAsync(o => o.UserId == user.UserId);
        if (otpRecord != null && otpRecord.Otp == otp && otpRecord.OtpExpiry > DateTime.UtcNow)
        {
            user.IsEmailVerified = true;
            _context.OtpVerifications.Remove(otpRecord);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }
}
