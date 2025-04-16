using BankApi.Data;
using BankApi.Entities;
using BankApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankApi.Repositories
{
    public class BankRepository : IBankRepository
    {
        private readonly BankDb1Context _context;

        public BankRepository(BankDb1Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Bank>> GetAllBanksAsync()
        {
            return await _context.Banks.ToListAsync();
        }

        public async Task<Bank> GetBankByIdAsync(int bankId)
        {
            return await _context.Banks.FindAsync(bankId);
        }

        public async Task<Bank> CreateBankAsync(Bank bank)
        {
            _context.Banks.Add(bank);
            await _context.SaveChangesAsync();
            return bank;
        }

        public async Task<Bank> UpdateBankAsync(Bank bank)
        {
            _context.Banks.Update(bank);
            await _context.SaveChangesAsync();
            return bank;
        }

        public async Task<bool> DeleteBankAsync(int bankId)
        {
            var bank = await GetBankByIdAsync(bankId);
            if (bank != null)
            {
                _context.Banks.Remove(bank);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
