using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using BankApi.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using BankApi.Dto;

namespace BankApi.Repositories
{
    public class BankManagerRepository : IBankManagerRepository
    {
        private readonly string _connectionString;

        public BankManagerRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public async Task<int> GetTotalTransactionCount()
        {
            using (var connection = CreateConnection())
            {
                return await connection.ExecuteScalarAsync<int>("sp_GetTotalTransactionCount", commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Transactions>> GetAllTransactions()
        {
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<Transactions>("sp_GetAllTransactions", commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Transactions>> GetDepositTransactions()
        {
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<Transactions>("sp_GetDepositTransactions", commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Transactions>> GetWithdrawTransactions()
        {
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<Transactions>("sp_GetWithdrawTransactions", commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<decimal> GetTotalAmountDeposited()
        {
            using (var connection = CreateConnection())
            {
                return await connection.ExecuteScalarAsync<decimal>("sp_GetTotalAmountDeposited", commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<decimal> GetTotalAmountWithdrawn()
        {
            using (var connection = CreateConnection())
            {
                return await connection.ExecuteScalarAsync<decimal>("sp_GetTotalAmountWithdrawn", commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<decimal> GetTotalBankBalance()
        {
            using (var connection = CreateConnection())
            {
                return await connection.ExecuteScalarAsync<decimal>("sp_GetTotalBankBalance", commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<TransactionDetails>> GetAllTransactionsWithDetails()
        {
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<TransactionDetails>("sp_GetAllTransactionsWithDetails", commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<UserTransactionSummary>> GetTotalAmountPerUser()
        {
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<UserTransactionSummary>("sp_GetTotalAmountPerUser", commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<BankManagerOverview> GetBankManagerOverview()
        {
            using (var connection = CreateConnection())
            {
                return await connection.QuerySingleAsync<BankManagerOverview>("sp_GetBankManagerOverview", commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<UserTransactionHistory>> GetUserTransactionHistory(int userId)
        {
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<UserTransactionHistory>(
                    "sp_GetUserTransactionHistory",
                    new { UserID = userId },
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        public async Task<IEnumerable<AccountDetails>> GetAllAccountsWithUserDetails()
        {
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<AccountDetails>("sp_GetAllAccountsWithUserDetails", commandType: CommandType.StoredProcedure);
            }
        }
    }
}
