using BankApi.Dto;
using BankApi.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankApi.Repositories.Interfaces
{
    public interface IBankManagerRepository
    {
        // Transaction Methods
        //Task<IEnumerable<TransactionResponseDto>> GetAllTransactions();
        //Task<IEnumerable<TransactionResponseDto>> GetTransactionsByType(string transactionType);
        //Task<IEnumerable<TransactionResponseDto>> GetTransactionsByTypeAndDateRange(string transactionType, DateTime? startDate, DateTime? endDate);
        //Task<IEnumerable<TransactionResponseDto>> GetTransactionsByDateRange(DateTime startDate, DateTime endDate);
        //Task<IEnumerable<TransactionResponseDto>> GetUserTransactions(int? userId, string? email);
        //Task<IEnumerable<TransactionResponseDto>> GetUserTransactionsByDateRange(int userId, DateTime startDate, DateTime endDate);

        // User & Account Details
        Task<AccountDetails> GetUserAccountDetails(int userId);
        Task<AccountDetails> GetUserDetailsByAccountNumber(string accountNumber);
        Task<AccountDetails> GetUserDetailsByEmail(string email);

        // Bank Summary
        Task<BankSummaryDto> GetAllTimeBankBalanceSheet();  // Added as a useful summary method
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
