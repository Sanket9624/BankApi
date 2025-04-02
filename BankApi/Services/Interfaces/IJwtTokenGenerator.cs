namespace BankApi.Services.Interfaces
{
    public interface IJwtTokenGenerator
    {
        Task<string> GenerateToken(int userId, string email, int roleId);
    }
}