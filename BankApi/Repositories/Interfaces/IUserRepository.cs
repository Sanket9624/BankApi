using System.Collections.Generic;
using System.Threading.Tasks;
using BankApi.Entities;

public interface IUserRepository
{
    Task<Account> GetAccountByIdAsync(int accountId);
    Task<Users> GetUserByIdAsync(int userId);
    Task<Account> GetAccountByNumberAsync(string accountNumber);
    Task<bool> DepositAsync(int accountId, decimal amount);
    Task<bool> WithdrawAsync(int accountId, decimal amount);
    Task<bool> TransferAsync(int senderAccountId, string receiverAccountNumber, decimal amount);
    Task<decimal> GetBalanceAsync(int accountId);
    Task<List<Transactions>> GetTransactionHistoryAsync(int accountId);
    Task<List<Transactions>> GetCustomeTransactionHistoryAsync(int accountId, DateTime? startDate, DateTime? endDate, TransactionType? type);
}
