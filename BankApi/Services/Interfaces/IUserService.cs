using System.Collections.Generic;
using System.Threading.Tasks;
using BankApi.Entities;

public interface IUserService
{
    Task<bool> DepositAsync(int userId, decimal amount, string description);
    Task<bool> WithdrawAsync(int userId, decimal amount, string description);
    Task<bool> TransferAsync(int userId, string receiverAccountNumber, decimal amount, string description);
    Task<BalanceDto> GetBalanceAsync(int userId);
    Task<List<TransactionDto>> GetCustomTransactionHistoryAsync(int userId, DateTime? startDate, DateTime? endDate, TransactionType? type,TransactionStatus? status);
}
