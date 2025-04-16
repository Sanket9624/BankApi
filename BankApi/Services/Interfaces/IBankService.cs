using BankApi.Dto.Request;
using BankApi.Dto.Response;

namespace BankApi.Services.Interfaces
{
    public interface IBankService
    {
        Task<IEnumerable<BankResponseDto>> GetAllBanksAsync();
        Task<BankResponseDto> GetBankByIdAsync(int bankId);
        Task<BankResponseDto> CreateBankAsync(BankRequestDto dto);
        Task<BankResponseDto> UpdateBankAsync(int bankId, BankRequestDto dto);
        Task<bool> DeleteBankAsync(int bankId);
    }
}
