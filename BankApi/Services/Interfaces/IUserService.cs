using System.Collections.Generic;
using System.Threading.Tasks;
using BankApi.Entities;

public interface IUserService
{
    Task<bool> DepositAsync(int userId, decimal amount);
    Task<bool> WithdrawAsync(int userId, decimal amount);
    Task<bool> TransferAsync(int userId, string receiverAccountNumber, decimal amount);
    Task<BalanceDto> GetBalanceAsync(int userId);
    Task<List<TransactionDto>> GetTransactionHistoryAsync(int userId);
    Task<List<TransactionDto>> GetCustomTransactionHistoryAsync(int userId, DateTime? startDate, DateTime? endDate, TransactionType? type);
}
