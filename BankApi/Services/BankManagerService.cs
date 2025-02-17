using BankApi.Dto;
using BankApi.Entities;
using BankApi.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankApi.Services
{
    public class BankManagerService : IBankManagerService
    {
        private readonly IBankManagerRepository _bankManagerRepository;

        public BankManagerService(IBankManagerRepository bankManagerRepository)
        {
            _bankManagerRepository = bankManagerRepository;
        }

        public async Task<int> GetTotalTransactionCount() => await _bankManagerRepository.GetTotalTransactionCount();
        public async Task<IEnumerable<Transactions>> GetAllTransactions() => await _bankManagerRepository.GetAllTransactions();
        public async Task<IEnumerable<Transactions>> GetDepositTransactions() => await _bankManagerRepository.GetDepositTransactions();
        public async Task<IEnumerable<Transactions>> GetWithdrawTransactions() => await _bankManagerRepository.GetWithdrawTransactions();
        public async Task<decimal> GetTotalAmountDeposited() => await _bankManagerRepository.GetTotalAmountDeposited();
        public async Task<decimal> GetTotalAmountWithdrawn() => await _bankManagerRepository.GetTotalAmountWithdrawn();
        public async Task<decimal> GetTotalBankBalance() => await _bankManagerRepository.GetTotalBankBalance();
        public async Task<IEnumerable<TransactionDetails>> GetAllTransactionsWithDetails() => await _bankManagerRepository.GetAllTransactionsWithDetails();
        public async Task<IEnumerable<UserTransactionSummary>> GetTotalAmountPerUser() => await _bankManagerRepository.GetTotalAmountPerUser();
        public async Task<BankManagerOverview> GetBankManagerOverview() => await _bankManagerRepository.GetBankManagerOverview();
        public async Task<IEnumerable<UserTransactionHistory>> GetUserTransactionHistory(int userId) => await _bankManagerRepository.GetUserTransactionHistory(userId);
        public async Task<IEnumerable<AccountDetails>> GetAllAccountsWithUserDetails() => await _bankManagerRepository.GetAllAccountsWithUserDetails();
    }
}
