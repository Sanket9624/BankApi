using BankApi.Dto;
using BankApi.Entities;

namespace BankApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterUserAsync(UserRequestDto ususerRequestDto);
        Task<string> LoginAsync(LoginDto loginDto);
        Task<UserWithAccountDto> GetUserByIdAsync(int userId);
    }
}