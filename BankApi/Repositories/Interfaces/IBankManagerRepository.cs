using BankApi.Dto;
using BankApi.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankApi.Repositories.Interfaces
{
    public interface IBankManagerRepository
    {

        // User & Account Details
        Task<AccountDetails> GetUserAccountDetails(int userId);
        Task<AccountDetails> GetUserDetailsByAccountNumber(string accountNumber);
        Task<AccountDetails> GetUserDetailsByEmail(string email);

        // Bank Summary
        Task<BankSummaryDto> GetAllTimeBankBalanceSheet();  // Added as a useful summary method
        Task<AccountSummaryDto> GetTotalAccounts();
        Task<int> GetTotalAccountCount();
        Task<IEnumerable<TransactionResponseDto>> GetTransactions(
           int? userId,
           string? transactionType,
           TransactionStatus? status,
           DateTime? startDate,
           DateTime? endDate);

    }
}
