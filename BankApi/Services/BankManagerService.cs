using BankApi.Dto;
using BankApi.Repositories.Interfaces;
using BankApi.Services.Interfaces;

namespace BankApi.Services
{
    public class BankManagerService : IBankManagerService
    {
        private readonly IBankManagerRepository _bankManagerRepository;

        public BankManagerService(IBankManagerRepository bankManagerRepository)
        {
            _bankManagerRepository = bankManagerRepository;
        }

        public async Task<BankSummaryDto> GetAllTimeBankBalanceSheet()
        {
            return await _bankManagerRepository.GetAllTimeBankBalanceSheet();
        }

        public async Task<AccountDetails> GetUserAccountDetails(int userId)
        {
            return await _bankManagerRepository.GetUserAccountDetails(userId);
        }

        public async Task<AccountDetails> GetUserDetailsByAccountNumber(string accountNumber)
        {
            return await _bankManagerRepository.GetUserDetailsByAccountNumber(accountNumber);
        }

        public async Task<AccountDetails> GetUserDetailsByEmail(string email)
        {
            return await _bankManagerRepository.GetUserDetailsByEmail(email);
        }
        public async Task<AccountSummaryDto> GetTotalAccounts()
        {
            return await _bankManagerRepository.GetTotalAccounts();
        }

        public async Task<int> GetTotalAccountCount()
        {
            return await _bankManagerRepository.GetTotalAccountCount();
        }
        public async Task<IEnumerable<TransactionResponseDto>> GetTransactions(
             int? userId = null,
             string? transactionType = null,
             DateTime? startDate = null,
             DateTime? endDate = null)
        {
            return await _bankManagerRepository.GetTransactions(userId, transactionType, startDate, endDate);
        }

    }
}
