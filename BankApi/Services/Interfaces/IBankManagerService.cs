using BankApi.Dto;

namespace BankApi.Services.Interfaces
{
    public interface IBankManagerService
    {
        Task<BankSummaryDto> GetAllTimeBankBalanceSheet();

        //Task<IEnumerable<TransactionResponseDto>> GetAllTransactions();
        //Task<IEnumerable<TransactionResponseDto>> GetTransactionsByType(string transactionType);
        //Task<IEnumerable<TransactionResponseDto>> GetTransactionsByTypeAndDateRange(string transactionType, DateTime? startDate, DateTime? endDate);
        //Task<IEnumerable<TransactionResponseDto>> GetTransactionsByDateRange(DateTime startDate, DateTime endDate);
        //Task<IEnumerable<TransactionResponseDto>> GetUserTransactions(int? userId, string? email);
        //Task<IEnumerable<TransactionResponseDto>> GetUserTransactionsByDateRange(int userId, DateTime startDate, DateTime endDate);
        Task<AccountDetails> GetUserAccountDetails(int userId);
        Task<AccountDetails> GetUserDetailsByAccountNumber(string accountNumber);
        Task<AccountDetails> GetUserDetailsByEmail(string email);
        Task<AccountSummaryDto> GetTotalAccounts();
        Task<int> GetTotalAccountCount();
        Task<IEnumerable<TransactionResponseDto>> GetTransactions(
                int? userId = null,
                string? transactionType = null,
                DateTime? startDate = null,
                DateTime? endDate = null
            );
        }
}
