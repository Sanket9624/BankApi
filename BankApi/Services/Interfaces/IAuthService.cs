using BankApi.Dto;
using BankApi.Entities;

namespace BankApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterUserAsync(UserRequestDto userRequestDto);
        Task<string> LoginAsync(LoginDto loginDto);
        Task<string> ToggleTwoFactorAsync(int userId, bool isEnabled);
        Task<UserWithAccountDto> GetUserByIdAsync(int userId);
        Task<string> SendOtpAsync(string email, int userId);
        Task<string> ForgotPasswordAsync(string email);
        Task<string> ResetPasswordAsync(string email, string otp, string newPassword);
        Task<object> VerifyOtpAsync(string email, string otp, string flowType , AccountType accountType);
        Task<bool> GetTwoFactorStatusAsync(int userId);
    }
}
