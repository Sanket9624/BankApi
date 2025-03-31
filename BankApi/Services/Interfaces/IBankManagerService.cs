using System.Transactions;
using BankApi.Dto;
using BankApi.Entities;

namespace BankApi.Services.Interfaces
{
    public interface IBankManagerService
    {
        Task<BankSummaryDto> GetAllTimeBankBalanceSheet();
        Task<AccountDetails> GetUserAccountDetails(int userId);
        Task<AccountDetails> GetUserDetailsByAccountNumber(string accountNumber);
        Task<AccountDetails> GetUserDetailsByEmail(string email);
        Task<AccountSummaryDto> GetTotalAccounts();
        Task<int> GetTotalAccountCount();
        Task<IEnumerable<TransactionResponseDto>> GetTransactions(
             int? userId,
             string? transactionType,
             BankApi.Entities.TransactionStatus? status,
             DateTime? startDate,
             DateTime? endDate);
        }
}
