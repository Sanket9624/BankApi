using BankApi.Dto.Request;
using BankApi.Dto.Response;

namespace BankApi.Services.Interfaces
{
    public interface IBranchService
    {
        Task<IEnumerable<BranchResponseDto>> GetAllBranchesAsync();
        Task<BranchResponseDto> GetBranchByIdAsync(int branchId);
        Task<IEnumerable<BranchResponseDto>> GetBranchesByBankIdAsync(int bankId);
        Task<BranchResponseDto> CreateBranchAsync(BranchRequestDto dto);
        Task<BranchResponseDto> UpdateBranchAsync(int branchId, BranchRequestDto dto);
        Task<bool> DeleteBranchAsync(int branchId);
    }
}
