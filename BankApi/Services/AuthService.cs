using AutoMapper;
using BankApi.Dto;
using BankApi.Entities;
using BankApi.Repositories.Interfaces;
using BankApi.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace BankApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly Repositories.Interfaces.IAuthRepository _authRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public AuthService(Repositories.Interfaces.IAuthRepository authRepository, IJwtTokenGenerator jwtTokenGenerator, IMapper mapper , IEmailService emailService)
        {
            _authRepository = authRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _mapper = mapper;
            _emailService = emailService;
        }

        public async Task<string> RegisterUserAsync(UserRequestDto userRequestDto)
        {
            var existingUser = await _authRepository.GetUserByEmailAsync(userRequestDto.Email);
            if (existingUser != null)
            {
                throw new Exception("User already exists");
            }

            var user = _mapper.Map<Users>(userRequestDto);
            user.PasswordHash = HashPassword(userRequestDto.Password);
            user.RoleId = 3;

            await _authRepository.CreateUserAsync(user);
            await SendOtpAsync(user.Email, user.UserId);

            return "OTP Sent to Email for Verification";
        }

        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            var user = await _authRepository.GetUserByEmailAsync(loginDto.Email);
            if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                throw new Exception("Invalid email or password");
            }

            if (!user.IsEmailVerified)
            {
                throw new Exception("User is not verified. Please verify your email.");
            }

            if (user.TwoFactorEnabled)
            {
                await SendOtpAsync(user.Email, user.UserId);
                return "OTP Sent for Verification. Please verify OTP to proceed.";
            }

            return _jwtTokenGenerator.GenerateToken(user.UserId, user.Email, user.RoleId);
        }

        public async Task<object> VerifyOtpAsync(string email, string otp, string flowType, AccountType accountType)
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

            if (flowType == "registration" || flowType == "login")
            {
                user.IsEmailVerified = true;
                user.Otp = null;
                user.OtpExpiry = null;
                await _authRepository.UpdateUserAsync(user);

                if (flowType == "registration")
                {
                    var accountNumber = GenerateAccountNumber();
                    var initialBalance = accountType == AccountType.Savings ? 1000 : 5000;

                    var account = new Account
                    {
                        UserId = user.UserId,
                        AccountNumber = accountNumber,
                        Balance = initialBalance,
                        AccountType = accountType,
                    };

                    await _authRepository.CreateAccountAsync(account);

                    return new { isSuccess = true, redirectType = "registration" };
                }

                if (flowType == "login")
                {
                    var token = _jwtTokenGenerator.GenerateToken(user.UserId, user.Email, user.RoleId);
                    return new { isSuccess = true, token, redirectType = "login" };
                }
            }
            else if (flowType == "forgotPassword")
            {
                user.IsEmailVerified = true;
                await _authRepository.UpdateUserAsync(user);

                return new { isSuccess = true, redirectType = "forgotPassword" };
            }

            throw new Exception("Invalid flow type");
        }

        public async Task<UserWithAccountDto> GetUserByIdAsync(int userId)
        {
            var user = await _authRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return null;
            }
            return _mapper.Map<UserWithAccountDto>(user);
        }

        private string GenerateOtp()
        {
            return new Random().Next(100000, 999999).ToString();
        }

        public async Task<string> SendOtpAsync(string email, int userId)
        {
            string otp = GenerateOtp();
            DateTime expiry = DateTime.UtcNow.AddMinutes(10);
            await _authRepository.SaveOtpAsync(userId, otp, expiry);

            var subject = "Your OTP Code";
            var body = $"Your OTP code is {otp}. It will expire in 10 minutes.";

            bool isEmailSent = await _emailService.SendEmailAsync(email, subject, body);

            return isEmailSent ? "OTP Sent Successfully" : "Failed to Send OTP";
        }

        public async Task<string> ForgotPasswordAsync(string email)
        {
            var user = await _authRepository.GetUserByEmailAsync(email);
            if (user == null) throw new Exception("User not found");

            await SendOtpAsync(email, user.UserId);
            return "OTP Sent for Password Reset";
        }

        public async Task<string> ResetPasswordAsync(string email, string otp, string newPassword)
        {
            var user = await _authRepository.GetUserByEmailAsync(email);
            if (user == null) throw new Exception("User not found");

            if (user.Otp != otp || user.OtpExpiry < DateTime.UtcNow)
            {
                throw new Exception("Invalid or Expired OTP");
            }

            user.PasswordHash = HashPassword(newPassword);
            user.Otp = null;
            user.OtpExpiry = null;

            await _authRepository.UpdateUserAsync(user);
            return "Password Reset Successfully";
        }

        public async Task<string> ToggleTwoFactorAsync(int userId, bool isEnabled)
        {
            var user = await _authRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            user.TwoFactorEnabled = isEnabled;
            await _authRepository.UpdateUserAsync(user);

            return isEnabled ? "Two-Factor Authentication Enabled" : "Two-Factor Authentication Disabled";
        }
        public async Task<bool> GetTwoFactorStatusAsync(int userId)
        {
            var user = await _authRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            return user.TwoFactorEnabled;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            return HashPassword(password) == storedHash;
        }

        private string GenerateAccountNumber()
        {
            return "304801000" + new Random().Next(10000, 99999);
        }
    }
}
