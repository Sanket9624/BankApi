using System.Collections.Generic;
using System.Threading.Tasks;
using BankApi.Entities;

public interface IUserRepository
{
    Task<Account> GetAccountByIdAsync(int accountId);
    Task<Users> GetUserByIdAsync(int userId);
    Task<Account> GetAccountByNumberAsync(string accountNumber);
   Task<bool> RequestDepositAsync(int accountId, decimal amount, string description, TransactionStatus status);
     Task<bool> RequestWithdrawAsync(int accountId, decimal amount, string description, TransactionStatus status);
     Task<bool> RequestTransferAsync(int senderAccountId, string receiverAccountNumber, decimal amount, string description,TransactionStatus status);
    Task<decimal> GetBalanceAsync(int accountId);
    Task<List<Transactions>> GetCustomeTransactionHistoryAsync(int accountId, DateTime? startDate, DateTime? endDate, TransactionType? type,TransactionStatus? status);
}
