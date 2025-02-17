using BankApi.Dto;
using BankApi.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankApi.Repositories
{
    public interface IBankManagerRepository
    {
        Task<int> GetTotalTransactionCount();
        Task<IEnumerable<Transactions>> GetAllTransactions();
        Task<IEnumerable<Transactions>> GetDepositTransactions();
        Task<IEnumerable<Transactions>> GetWithdrawTransactions();
        Task<decimal> GetTotalAmountDeposited();
        Task<decimal> GetTotalAmountWithdrawn();
        Task<decimal> GetTotalBankBalance();
        Task<IEnumerable<TransactionDetails>> GetAllTransactionsWithDetails();
        Task<IEnumerable<UserTransactionSummary>> GetTotalAmountPerUser();
        Task<BankManagerOverview> GetBankManagerOverview();
        Task<IEnumerable<UserTransactionHistory>> GetUserTransactionHistory(int userId);
        Task<IEnumerable<AccountDetails>> GetAllAccountsWithUserDetails();
    }
}
