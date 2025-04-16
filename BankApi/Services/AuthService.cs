using AutoMapper;
using BankApi.Data;
using BankApi.Dto;
using BankApi.Entities;
using BankApi.Repositories.Interfaces;
using BankApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace BankApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly BankDb1Context _context;

        public AuthService(IAuthRepository authRepository, IJwtTokenGenerator jwtTokenGenerator, IMapper mapper, IEmailService emailService, BankDb1Context context)
        {
            _authRepository = authRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _mapper = mapper;
            _emailService = emailService;
            _context = context;
        }

    //Auth Methods
        //1.Register Logic
        public async Task<string> RegisterUserAsync(UserRequestDto userRequestDto)
        {
            if (await _authRepository.GetUserByEmailAsync(userRequestDto.Email) != null)
                throw new Exception("User already exists");

            var defaultRole = await _authRepository.GetRoleByNameAsync("Customer")
                              ?? throw new Exception("Default role not found");

            var user = _mapper.Map<Users>(userRequestDto);
            user.PasswordHash = PasswordUtility.HashPassword(userRequestDto.Password);
            user.RoleId = defaultRole.RoleId;
            user.AccountType = userRequestDto.AccountType;  

            if (user.IsEmailVerified)
            {
                user.RequestStatus = RequestStatus.Pending;
            }
            await _authRepository.CreateUserAsync(user);
            await SendVerificationEmailAsync(user.Email, user.UserId);

            return "Verification email sent. Please check your inbox.";
        }

        //2.Login Logic
        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            var user = await _authRepository.GetUserByEmailAsync(loginDto.Email);
            if (user == null || !PasswordUtility.VerifyPassword(loginDto.Password, user.PasswordHash))
                throw new Exception("Invalid email or password");

            if (!user.IsEmailVerified) throw new Exception("User is not verified. Please verify your email.");
            if (user.RequestStatus != RequestStatus.Approved) throw new Exception("Your account is pending approval by an admin.");

            if (user.TwoFactorEnabled)
            {
                await SendVerificationEmailAsync(user.Email, user.UserId);
                Console.WriteLine("message send:");
                return "OTP Sent for Verification. Please verify OTP to proceed.";
            }

            return _jwtTokenGenerator.GenerateToken(user.UserId, user.Email, user.RoleId);
        }

    //Get User Detail By Id 
        public async Task<UserWithAccountDto> GetUserByIdAsync(int userId)
        {
            var user = await _authRepository.GetUserByIdAsync(userId);
            return user == null ? null : _mapper.Map<UserWithAccountDto>(user);
        }

     //Forgot and Reset Password

        //1.Forgot Password
        public async Task<string> ForgotPasswordAsync(string email)
        {
            var user = await _authRepository.GetUserByEmailAsync(email);
            if (user == null) throw new Exception("User not found");

            await SendVerificationEmailAsync(email, user.UserId);
            return "OTP Sent for Password Reset";
        } 

        //2.Reset Password
        public async Task<bool> ResetPasswordAsync(string email, string newPassword)
        {
            var user = await _authRepository.GetUserByEmailAsync(email)
                       ?? throw new Exception("User not found");

            user.PasswordHash = PasswordUtility.HashPassword(newPassword);

            // Remove OTP after password reset
            var otpRecord = await _context.OtpVerifications.FirstOrDefaultAsync(o => o.UserId == user.UserId);
            if (otpRecord != null)
            {
                _context.OtpVerifications.Remove(otpRecord);
            }

            await _authRepository.UpdateUserAsync(user);
            await _context.SaveChangesAsync();

            return true;
        }

    //Email Verification and 2fa Logic

        //1.Send verification otp in mail using link
        public async Task<string> SendVerificationEmailAsync(string email, int userId)
        {
            string otp = OtpUtility.GenerateOtp();
            DateTime expiry = OtpUtility.GetOtpExpiry();
            await _authRepository.SaveOtpAsync(userId, otp, expiry);

            // Open signup page and trigger OTP modal automatically
            string verificationLink = $"http://localhost:5173/signup?otp={otp}&email={email}";

            string emailBody = $"Your otp is {otp}";

            Console.WriteLine("emailBody",emailBody);
            bool isEmailSent = await _emailService.SendEmailAsync(email, "Verify Your Email", emailBody);
            if (isEmailSent)
            {
                Console.WriteLine("Verification email sent successfully");
            }
            else
            {
                Console.WriteLine("Failed to send verification email");
            }
                return isEmailSent ? "Verification email sent successfully" : "Failed to send verification email";
        }

        //2Verify Otp For Different Cases
        public async Task<object> VerifyOtpAsync(string email, string otp, string flowType)
        {
            var user = await _authRepository.GetUserByEmailAsync(email)
                       ?? throw new Exception("User not found");

            if (!await _authRepository.VerifyOtpAsync(email, otp))
                throw new Exception("Invalid or expired OTP");

            // Remove OTP only for registration or login
            if (flowType == "registration" || flowType == "login")
            {
                user.IsEmailVerified = true;
                var otpRecord = await _context.OtpVerifications.FirstOrDefaultAsync(o => o.UserId == user.UserId);
                if (otpRecord != null)
                {
                    _context.OtpVerifications.Remove(otpRecord);
                    await _context.SaveChangesAsync();
                }
            }

            return flowType switch
            {
                "registration" => new { isSuccess = true, message = "Email verified successfully. Waiting for Admin Approval." },
                "login" => user.RequestStatus == RequestStatus.Approved
                    ? new { isSuccess = true, token = _jwtTokenGenerator.GenerateToken(user.UserId, user.Email, user.RoleId), redirectType = "login" }
                    : throw new Exception("Your account is pending approval by an admin."),
                "forgotPassword" => new { isSuccess = true, redirectType = "forgotPassword" }, // OTP remains
                _ => throw new Exception("Invalid flow type")
            };
        }


        //3.Two Factor Authentication enabled and disabled logic 
        public async Task<string> ToggleTwoFactorAsync(int userId, bool isEnabled)
        {
            var user = await _authRepository.GetUserByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            user.TwoFactorEnabled = isEnabled;
            await _authRepository.UpdateUserAsync(user);

            return isEnabled ? "Two-Factor Authentication Enabled" : "Two-Factor Authentication Disabled";
        }

        //4.Get Status of Two Factor Authentication
        public async Task<bool> GetTwoFactorStatusAsync(int userId)
        {
            var user = await _authRepository.GetUserByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            return user.TwoFactorEnabled;
        }
    }
}
