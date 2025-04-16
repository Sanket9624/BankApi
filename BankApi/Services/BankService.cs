using BankApi.Dto.Request;
using BankApi.Dto.Response;
using BankApi.Entities;
using BankApi.Repositories.Interfaces;
using BankApi.Services.Interfaces;

namespace BankApi.Services
{
    public class BankService : IBankService
    {
        private readonly IBankRepository _bankRepository;

        public BankService(IBankRepository bankRepository)
        {
            _bankRepository = bankRepository;
        }

        public async Task<IEnumerable<BankResponseDto>> GetAllBanksAsync()
        {
            var banks = await _bankRepository.GetAllBanksAsync();
            return banks.Select(b => new BankResponseDto
            {
                BankId = b.BankId,
                Name = b.Name,
                Address = b.Address
            });
        }

        public async Task<BankResponseDto> GetBankByIdAsync(int bankId)
        {
            var bank = await _bankRepository.GetBankByIdAsync(bankId);
            return bank == null ? null : new BankResponseDto
            {
                BankId = bank.BankId,
                Name = bank.Name,
                Address = bank.Address
            };
        }

        public async Task<BankResponseDto> CreateBankAsync(BankRequestDto dto)
        {
            var bank = new Bank
            {
                Name = dto.Name,
                Address = dto.Address
            };

            var created = await _bankRepository.CreateBankAsync(bank);
            return new BankResponseDto
            {
                BankId = created.BankId,
                Name = created.Name,
                Address = created.Address
            };
        }

        public async Task<BankResponseDto> UpdateBankAsync(int bankId, BankRequestDto dto)
        {
            var existing = await _bankRepository.GetBankByIdAsync(bankId);
            if (existing == null) return null;

            existing.Name = dto.Name;
            existing.Address = dto.Address;

            var updated = await _bankRepository.UpdateBankAsync(existing);
            return new BankResponseDto
            {
                BankId = updated.BankId,
                Name = updated.Name,
                Address = updated.Address
            };
        }

        public async Task<bool> DeleteBankAsync(int bankId)
        {
            return await _bankRepository.DeleteBankAsync(bankId);
        }
    }
}
