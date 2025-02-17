using AutoMapper;
using BankApi.Dto;
using BankApi.Entities;
using BankApi.Repositories.Interfaces;
using BankApi.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace BankApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator, IMapper mapper)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _mapper = mapper;
        }

        public async Task<string> RegisterUserAsync(UserRequestDto userRequestDto)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(userRequestDto.Email);
            if (existingUser != null)
            {
                throw new Exception("User already exists");
            }

            var user = _mapper.Map<Users>(userRequestDto);
            user.PasswordHash = HashPassword(userRequestDto.Password);
            user.RoleId = 3;
            user.Account = new Account
            {
                AccountNumber = GenerateAccountNumber(),
                Balance = userRequestDto.AccountType == AccountType.Savings ? 1000 : 5000,
                AccountType = userRequestDto.AccountType
            };

            await _userRepository.CreateUserAsync(user);
            return "User registered successfully";
        }

        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);
            if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                throw new Exception("Invalid email or password");
            }

            return _jwtTokenGenerator.GenerateToken(user.UserId, user.Email, user.RoleId);
        }

        public async Task<UserWithAccountDto> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return null;
            }
            return _mapper.Map<UserWithAccountDto>(user);
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
