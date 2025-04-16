using BankApi.Dto.Request;
using BankApi.Dto.Response;
using BankApi.Entities;
using BankApi.Repositories.Interfaces;
using BankApi.Services.Interfaces;

namespace BankApi.Services
{
    public class BranchService : IBranchService
    {
        private readonly IBranchRepository _branchRepository;

        public BranchService(IBranchRepository branchRepository)
        {
            _branchRepository = branchRepository;
        }

        public async Task<IEnumerable<BranchResponseDto>> GetAllBranchesAsync()
        {
            var branches = await _branchRepository.GetAllBranchesAsync();
            return branches.Select(b => new BranchResponseDto
            {
                BranchId = b.BranchId ?? 0,
                Name = b.Name,
                Address = b.Address,
                BankId = b.BankId
            });
        }

        public async Task<BranchResponseDto> GetBranchByIdAsync(int branchId)
        {
            var branch = await _branchRepository.GetBranchByIdAsync(branchId);
            return branch == null ? null : new BranchResponseDto
            {
                BranchId = branch.BranchId ?? 0,
                Name = branch.Name,
                Address = branch.Address,
                BankId = branch.BankId
            };
        }

        public async Task<IEnumerable<BranchResponseDto>> GetBranchesByBankIdAsync(int bankId)
        {
            var branches = await _branchRepository.GetBranchesByBankIdAsync(bankId);
            return branches.Select(b => new BranchResponseDto
            {
                BranchId = b.BranchId ?? 0,
                Name = b.Name,
                Address = b.Address,
                BankId = b.BankId
            });
        }

        public async Task<BranchResponseDto> CreateBranchAsync(BranchRequestDto dto)
        {
            var branch = new Branch
            {
                Name = dto.Name,
                Address = dto.Address,
                BankId = dto.BankId
            };

            var created = await _branchRepository.CreateBranchAsync(branch);
            return new BranchResponseDto
            {
                BranchId = created.BranchId ?? 0,
                Name = created.Name,
                Address = created.Address,
                BankId = created.BankId
            };
        }

        public async Task<BranchResponseDto> UpdateBranchAsync(int branchId, BranchRequestDto dto)
        {
            var existing = await _branchRepository.GetBranchByIdAsync(branchId);
            if (existing == null) return null;

            existing.Name = dto.Name;
            existing.Address = dto.Address;
            existing.BankId = dto.BankId;

            var updated = await _branchRepository.UpdateBranchAsync(existing);
            return new BranchResponseDto
            {
                BranchId = updated.BranchId ?? 0,
                Name = updated.Name,
                Address = updated.Address,
                BankId = updated.BankId
            };
        }

        public async Task<bool> DeleteBranchAsync(int branchId)
        {
            return await _branchRepository.DeleteBranchAsync(branchId);
        }
    }
}
