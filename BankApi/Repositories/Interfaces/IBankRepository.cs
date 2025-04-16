using BankApi.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankApi.Repositories.Interfaces
{
    public interface IBankRepository
    {
        Task<IEnumerable<Bank>> GetAllBanksAsync();
        Task<Bank> GetBankByIdAsync(int bankId);
        Task<Bank> CreateBankAsync(Bank bank);
        Task<Bank> UpdateBankAsync(Bank bank);
        Task<bool> DeleteBankAsync(int bankId);
    }
}
