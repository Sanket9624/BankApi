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

        //public async Task<IEnumerable<TransactionResponseDto>> GetAllTransactions()
        //{
        //    return await _bankManagerRepository.GetAllTransactions();
        //}

        //public async Task<IEnumerable<TransactionResponseDto>> GetTransactionsByType(string transactionType)
        //{
        //    return await _bankManagerRepository.GetTransactionsByType(transactionType);
        //}
        //public async Task<IEnumerable<TransactionResponseDto>> GetTransactionsByTypeAndDateRange(string transactionType, DateTime? startDate, DateTime? endDate)
        //{
        //    return await _bankManagerRepository.GetTransactionsByTypeAndDateRange(transactionType, startDate, endDate);
        //}

        //public async Task<IEnumerable<TransactionResponseDto>> GetTransactionsByDateRange(DateTime startDate, DateTime endDate)
        //{
        //    return await _bankManagerRepository.GetTransactionsByDateRange(startDate, endDate);
        //}

        //public async Task<IEnumerable<TransactionResponseDto>> GetUserTransactions(int? userId, string? email)
        //{
        //    return await _bankManagerRepository.GetUserTransactions(userId, email);
        //}

        //public async Task<IEnumerable<TransactionResponseDto>> GetUserTransactionsByDateRange(int userId, DateTime startDate, DateTime endDate)
        //{
        //    return await _bankManagerRepository.GetUserTransactionsByDateRange(userId, startDate, endDate);
        //}

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
