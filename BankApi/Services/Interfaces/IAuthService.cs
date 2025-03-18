using BankApi.Dto;
using System.Threading.Tasks;

namespace BankApi.Services.Interfaces
{
    public interface IAuthService
    {
    //Auth Service
        //Register
        Task<string> RegisterUserAsync(UserRequestDto userRequestDto);
        //Login
        Task<string> LoginAsync(LoginDto loginDto);

    //Get user own details
        Task<UserWithAccountDto> GetUserByIdAsync(int userId);

    //forgot Password 
        Task<string> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string newPassword);
        
    //For two factor authentication and otp verify
        Task<string> SendVerificationEmailAsync(string email, int userId);
        Task<object> VerifyOtpAsync(string email, string otp, string flowType);
        Task<string> ToggleTwoFactorAsync(int userId, bool isEnabled);
        Task<bool> GetTwoFactorStatusAsync(int userId);
    }
}
