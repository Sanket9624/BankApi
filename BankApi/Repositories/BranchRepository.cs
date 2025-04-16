using BankApi.Data;
using BankApi.Entities;
using BankApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankApi.Repositories
{
    public class BranchRepository : IBranchRepository
    {
        private readonly BankDb1Context _context;

        public BranchRepository(BankDb1Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Branch>> GetAllBranchesAsync()
        {
            return await _context.Branches.ToListAsync();
        }

        public async Task<Branch> GetBranchByIdAsync(int branchId)
        {
            return await _context.Branches.FindAsync(branchId);
        }

        public async Task<IEnumerable<Branch>> GetBranchesByBankIdAsync(int bankId)
        {
            return await _context.Branches.Where(b => b.BankId == bankId).ToListAsync();
        }

        public async Task<Branch> CreateBranchAsync(Branch branch)
        {
            _context.Branches.Add(branch);
            await _context.SaveChangesAsync();
            return branch;
        }

        public async Task<Branch> UpdateBranchAsync(Branch branch)
        {
            _context.Branches.Update(branch);
            await _context.SaveChangesAsync();
            return branch;
        }

        public async Task<bool> DeleteBranchAsync(int branchId)
        {
            var branch = await GetBranchByIdAsync(branchId);
            if (branch != null)
            {
                _context.Branches.Remove(branch);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
