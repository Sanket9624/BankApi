using AutoMapper;
using BankApi.Dto;
using BankApi.Dto.Request;
using BankApi.Dto.Response;
using BankApi.Entities;
using BankApi.Repositories.Interfaces;
using BankApi.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankApi.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IAuthRepository _authRepository;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;

        public AdminService(
            IAdminRepository adminRepository,
            IMapper mapper,
            IAuthService authService,
            IAuthRepository authRepository,
            IEmailService emailService)
        {
            _adminRepository = adminRepository;
            _authRepository = authRepository;
            _mapper = mapper;
            _authService = authService;
            _emailService = emailService;
        }


    //Roles CRUD Opereation
        
        //Create Role
        public async Task<RoleRequestDto> CreateRoleAsync(string roleName)
        {
            var existingRole = await _authRepository.GetRoleByNameAsync(roleName);
            if (existingRole != null) throw new Exception("Role already exists.");

            var role = new RoleMaster { RoleName = roleName };
            var createdRole = await _adminRepository.CreateRoleAsync(role);
            return _mapper.Map<RoleRequestDto>(createdRole);
        }

        //Get Role
        public async Task<List<RoleResponseDto>> GetRolesAsync()
        {
            var roles = await _adminRepository.GetRolesAsync();
            return _mapper.Map<List<RoleResponseDto>>(roles);
        }

        //Delete Role
        public async Task<bool> DeleteRoleAsync(int roleId)
        {
            var role = await _adminRepository.GetRoleByIdAsync(roleId);
            if (role == null) throw new Exception("Role not found.");
            return await _adminRepository.DeleteRoleAsync(roleId);
        }

      
    //Manager CRUD

        //Create A Bank Manager
        public async Task<BankManagerDto> CreateBankManagerAsync(BankManagerDto bankManagerDto)
        {
            var existingUser = await _authRepository.GetUserByEmailAsync(bankManagerDto.Email);
            if (existingUser != null) throw new Exception("User already exists.");

            var bankManager = _mapper.Map<Users>(bankManagerDto);
            bankManager.PasswordHash = PasswordUtility.HashPassword(bankManagerDto.Password);
            bankManager.IsEmailVerified = false;

            var createdBankManager = await _adminRepository.CreateBankManagerAsync(bankManager);

            // Generate OTP and store it in the OTP table
            var otp = OtpUtility.GenerateOtp();
            var expiry = DateTime.UtcNow.AddHours(24);
            await _authRepository.SaveOtpAsync(createdBankManager.UserId, otp, expiry);

            // Send OTP via email
            await _emailService.SendEmailAsync(createdBankManager.Email, "Email Verification", $"Your OTP is: {otp}");

            return _mapper.Map<BankManagerDto>(createdBankManager);
        }
        
        //Get List Of Bank Managers
        public async Task<List<AdminResponseDto>> GetBankManagersAsync()
        {
            var bankManagers = await _adminRepository.GetBankManagersAsync();
            return _mapper.Map<List<AdminResponseDto>>(bankManagers);
        }

        //Verify Manager using otp
        public async Task<string> VerifyOtpAsync(string email, string otp)
        {
            var isValid = await _authRepository.VerifyOtpAsync(email, otp);
            if (!isValid) throw new Exception("Invalid or expired OTP.");

            var user = await _authRepository.GetUserByEmailAsync(email);
            if (user == null) throw new Exception("User not found.");

            user.IsEmailVerified = true;
            await _authRepository.UpdateUserAsync(user);


            await _emailService.SendEmailAsync(user.Email, "Your Login Credentials",
                $"Dear {user.FirstName + ' ' + user.LastName},\n\nYour email has been successfully verified. Here are your login credentials:\n\n" +
                $"Username: {user.Email}\nPassword: Use the Password You Provided During Registration\n\n" +
                "Please change your password after logging in.\n\nBest regards,\nBank Management Team");

            return "Manager Created and Credentials Sent Successfully";

        }

    //Customer 

        //Get the Customers List
        public async Task<List<UserResponseDto>> GetAllUsersExceptAdminAsync()
        {
            var users = await _adminRepository.GetAllUsersExceptAdminAsync();
            return _mapper.Map<List<UserResponseDto>>(users);
        }

        //Users CRUD

        //Approve Account Request Of User or Reject Request
        public async Task<string> ApproveAccountRequest(int userId, bool isApproved, AccountType accountType, string? rejectedReason = null)
        {
            var user = await _authRepository.GetUserByIdAsync(userId);
            if (user == null) throw new Exception("User not found.");
            if(user.IsEmailVerified == false)
            {
                throw new Exception("User Is not Verified for Generate Account");
            }

            user.RequestStatus = isApproved ? RequestStatus.Approved : RequestStatus.Rejected;

            if (!isApproved)
            {
                user.RejectionReason = rejectedReason;
                user.RejectedAt = DateTime.Now;

            }

            await _authRepository.UpdateUserAsync(user);

            if (isApproved)
            {
                var accountNumber = GenerateAccountNumber();
                var account = new Account
                {
                    UserId = user.UserId,
                    AccountNumber = accountNumber,
                    Balance = user.AccountType == AccountType.Savings ? 1000 : 5000, // Use the selected account type
                    AccountType =user.AccountType,
                };

                await _adminRepository.CreateAccountAsync(account);
                await _emailService.SendEmailAsync(user.Email, "Account Approved",
                    $"Your account has been approved! Your account number is {accountNumber}.");
            }
            else
            {
                await _emailService.SendEmailAsync(user.Email, "Account Rejected",
                    $"Your account request was rejected. Reason: {rejectedReason ?? "No specific reason provided."}");
            }

            return isApproved ? "Account Approved Successfully" : "Account Rejected";
        }

        //Generate AccountNumber 
        private string GenerateAccountNumber()
        {
            return "304801000" + new Random().Next(10000, 99999);
        }

        public async Task<(bool success, string errorMessage)> ApproveTransactionAsync(int transactionId)
        {
            var transaction = await _adminRepository.GetTransactionByIdAsync(transactionId);

            if (transaction == null)
                return (false, "Transaction not found.");

            if (transaction.Status != TransactionStatus.Pending)
                return (false, $"Transaction is not pending. Current status: {transaction.Status}");

            Console.WriteLine($"Transaction {transaction.TransactionId}: Type = {transaction.Type}, Amount = {transaction.Amount}");

            Console.WriteLine($"SenderAccountId: {(transaction.SenderAccountId.HasValue ? transaction.SenderAccountId.ToString() : "NULL")}");
            Console.WriteLine($"ReceiverAccountId: {(transaction.ReceiverAccountId.HasValue ? transaction.ReceiverAccountId.ToString() : "NULL")}");

            var sender = transaction.SenderAccountId.HasValue && transaction.SenderAccountId.Value > 0
                ? await _adminRepository.GetAccountByUserIdAsync(transaction.SenderAccountId.Value)
                : null;

            var receiver = transaction.ReceiverAccountId.HasValue && transaction.ReceiverAccountId.Value > 0
                ? await _adminRepository.GetAccountByUserIdAsync(transaction.ReceiverAccountId.Value)
                : null;

            Console.WriteLine($"Sender: {(sender != null ? sender.AccountId.ToString() : "NULL")}, Receiver: {(receiver != null ? receiver.AccountId.ToString() : "NULL")}");

            if (transaction.Type == TransactionType.Deposit)
            {
                if (receiver == null)
                    return (false, "Receiver account is missing for deposit.");

                receiver.Balance += transaction.Amount;
                await _adminRepository.UpdateAccountAsync(receiver);
            }
            else if (transaction.Type == TransactionType.Withdraw)
            {
                if (sender == null)
                    return (false, "Sender account is missing for withdrawal.");

                if (sender.Balance < transaction.Amount)
                    return (false, "Insufficient funds for withdrawal.");

                sender.Balance -= transaction.Amount;
                await _adminRepository.UpdateAccountAsync(sender);
            }
            else if (transaction.Type == TransactionType.Transfer)
            {
                if (sender == null || receiver == null)
                    return (false, "Both sender and receiver accounts are required for transfer.");

                if (sender.Balance < transaction.Amount)
                    return (false, "Insufficient funds for transfer.");

                sender.Balance -= transaction.Amount;
                receiver.Balance += transaction.Amount;
                await _adminRepository.UpdateAccountAsync(sender);
                await _adminRepository.UpdateAccountAsync(receiver);
            }
            else
            {
                return (false, "Invalid transaction type.");
            }

            transaction.Status = TransactionStatus.Approved;
            await _adminRepository.UpdateTransactionAsync(transaction);

            Console.WriteLine($"Transaction {transactionId} approved successfully.");
            return (true, null);
        }



        public async Task<bool> RejectTransactionAsync(int transactionId, string reason)
        {
            var transaction = await _adminRepository.GetTransactionByIdAsync(transactionId);
            if (transaction == null || transaction.Status != TransactionStatus.Pending)
                return false;

            transaction.Status = TransactionStatus.Rejected;
            transaction.Reason = reason;
            await _adminRepository.UpdateTransactionAsync(transaction);
            return true;
        }
        //Get The Users with Verify Email and Approve Account
        public async Task<IEnumerable<UserStatusDto>> GetAllUsersWithStatusAsync()
        {
            var users = await _adminRepository.GetAllUsersAsync();

            return users
                .Where(u => u.RequestStatus == RequestStatus.Pending && u.IsEmailVerified) // ✅ Filter pending & verified users
                .Select(u => new UserStatusDto
                {
                    UserId = u.UserId,
                    FullName = $"{u.FirstName} {u.LastName}",
                    Address = u.Address,
                    AccountType = u.AccountType,
                    Email = u.Email,
                    RequestStatus = u.RequestStatus,
                    IsEmailVerified = u.IsEmailVerified,
                    RejectionReason = u.RejectionReason
                });
        }


        //Update the Users
        public async Task<BankMangerUpdateDto> UpdateUserAsync(int userId, BankMangerUpdateDto dto)
        {
            var updatedUser = await _adminRepository.UpdateUserAsync(userId, dto);
            if (updatedUser == null) throw new Exception("User not found.");

            return _mapper.Map<BankMangerUpdateDto>(updatedUser);
        }
        //Soft Delete the Users
        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _authRepository.GetUserByIdAsync(userId);
            if (user == null) throw new Exception("User not found.");
            return await _adminRepository.DeleteUserAsync(userId);
        }
        public async Task<List<UserResponseDto>> GetApprovedAccountsAsync()
        {
            var users = await _adminRepository.GetApproveOrRejectedAccountsAsync();
            return _mapper.Map<List<UserResponseDto>>(users);
        }
        public async Task<List<TransactionResponseDto>> GetPendingTransactionsAsync()
        {
            var transactions = await _adminRepository.GetTransactionsByStatusAsync(TransactionStatus.Pending);
            return _mapper.Map<List<TransactionResponseDto>>(transactions);
        }
    }
}
