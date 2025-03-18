using Dapper;
using System.Data;
using Microsoft.Data.SqlClient;
using BankApi.Dto;
using BankApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Connections;

namespace BankApi.Repositories
{
    public class BankManagerRepository : IBankManagerRepository
    {
        private readonly IConfiguration _configuration;

        public BankManagerRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Create Database Connection
        private IDbConnection CreateConnection()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            return new SqlConnection(connectionString);
        }

        // Get User and Account Details
        public async Task<AccountDetails> GetUserAccountDetails(int userId)
        {
            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<AccountDetails>(
                "GetUserAccountDetails",
                new { UserId = userId },
                commandType: CommandType.StoredProcedure
            );
        }

        // Get User Details by Account Number
        public async Task<AccountDetails> GetUserDetailsByAccountNumber(string accountNumber)
        {
            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<AccountDetails>(
                "GetUserDetailsByAccountNumber",
                new { AccountNumber = accountNumber },
                commandType: CommandType.StoredProcedure
            );
        }

        // Get User Details by Email
        public async Task<AccountDetails> GetUserDetailsByEmail(string email)
        {
            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<AccountDetails>(
                "GetUserDetailsByEmail",
                new { Email = email },
                commandType: CommandType.StoredProcedure
            );
        }

        // Get Bank Summary
        public async Task<BankSummaryDto> GetAllTimeBankBalanceSheet()
        {
            using var connection = CreateConnection();

            using var multi = await connection.QueryMultipleAsync("GetAllTimeBankBalanceSheet", commandType: CommandType.StoredProcedure);

            var bankSummary = new BankSummaryDto
            {
                TotalBankBalance = await multi.ReadFirstOrDefaultAsync<decimal>(),
                TotalDepositedMoney = await multi.ReadFirstOrDefaultAsync<decimal>(),
                TotalWithdrawnMoney = await multi.ReadFirstOrDefaultAsync<decimal>(),
                TotalTransactions = await multi.ReadFirstOrDefaultAsync<int>(),
                UserTransactionCounts = (await multi.ReadAsync<UserTransactionCountDto>()).ToList()
            };

            return bankSummary;
        }
      public async Task<AccountSummaryDto> GetTotalAccounts()
        {
            using var connection = CreateConnection();
            using var multi = await connection.QueryMultipleAsync("GetTotalAccounts", commandType: CommandType.StoredProcedure);

            var totalAccounts = await multi.ReadFirstOrDefaultAsync<int>();
            var accountDetails = (await multi.ReadAsync<AccountDto>()).ToList();

            return new AccountSummaryDto
            {
                TotalAccounts = totalAccounts,
                AccountDetails = accountDetails
            };
        }

        public async Task<int> GetTotalAccountCount()
        {
            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<int>("SELECT COUNT(*) FROM Accounts");
        }
        public async Task<IEnumerable<TransactionResponseDto>> GetTransactions(
                 int? userId = null,
                 string? transactionType = null,
                 DateTime? startDate = null,
                 DateTime? endDate = null)
        {
            using var connection = CreateConnection();

            return await connection.QueryAsync<TransactionResponseDto>(
                "GetTransactions",
                new
                {
                    UserId = userId,
                    TransactionType = transactionType,
                    StartDate = startDate,
                    EndDate = endDate
                },
                commandType: CommandType.StoredProcedure
            );
        }

    }
}

