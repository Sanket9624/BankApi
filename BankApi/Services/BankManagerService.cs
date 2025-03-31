using System.Transactions;
using BankApi.Dto;
using BankApi.Repositories.Interfaces;
using BankApi.Services.Interfaces;
using BankApi.Entities;

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
          int? userId,
          string? transactionType,
          BankApi.Entities.TransactionStatus? status,
          DateTime? startDate,
          DateTime? endDate)
        {
            return await _bankManagerRepository.GetTransactions(userId, transactionType, status, startDate, endDate);
        }

    }
}
