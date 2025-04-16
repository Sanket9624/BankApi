using BankApi.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankApi.Repositories.Interfaces
{
    public interface IBranchRepository
    {
        Task<IEnumerable<Branch>> GetAllBranchesAsync();
        Task<Branch> GetBranchByIdAsync(int branchId);
        Task<IEnumerable<Branch>> GetBranchesByBankIdAsync(int bankId);
        Task<Branch> CreateBranchAsync(Branch branch);
        Task<Branch> UpdateBranchAsync(Branch branch);
        Task<bool> DeleteBranchAsync(int branchId);
    }
}
