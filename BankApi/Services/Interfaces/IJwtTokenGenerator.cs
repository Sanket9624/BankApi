namespace BankApi.Services.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(int userId, string email, int roleId);
    }
}