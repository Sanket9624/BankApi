using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankApi.Entities;

public class BankingService : IBankingService
{
    private readonly IBankingRepository _repository;

    public BankingService(IBankingRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> DepositAsync(int userId, decimal amount)
    {
        var user = await _repository.GetUserByIdAsync(userId);
        if (user?.Account == null) return false;

        return await _repository.DepositAsync(user.Account.AccountId, amount);
    }

    public async Task<bool> WithdrawAsync(int userId, decimal amount)
    {
        var user = await _repository.GetUserByIdAsync(userId);
        if (user?.Account == null) return false;

        return await _repository.WithdrawAsync(user.Account.AccountId, amount);
    }

    public async Task<bool> TransferAsync(int userId, string receiverAccountNumber, decimal amount)
    {
        var sender = await _repository.GetUserByIdAsync(userId);
        //var receiver = await _repository.GetAccountByIdAsync(userId);
        if (sender?.Account == null) return false;

        return await _repository.TransferAsync(sender.Account.AccountId, receiverAccountNumber, amount);
    }


    public async Task<BalanceDto> GetBalanceAsync(int userId)
    {
        var user = await _repository.GetUserByIdAsync(userId);
        if (user?.Account == null) return new BalanceDto { Balance = 0 };

        var balance = await _repository.GetBalanceAsync(user.Account.AccountId);
        return new BalanceDto { Balance = balance };
    }

    public async Task<List<TransactionDto>> GetTransactionHistoryAsync(int userId)
    {
        var user = await _repository.GetUserByIdAsync(userId);
        if (user?.Account == null) return new List<TransactionDto>();

        var transactions = await _repository.GetTransactionHistoryAsync(user.Account.AccountId);
        return transactions.Select(t => new TransactionDto
        {
            TransactionId = t.TransactionId,
            Amount = t.Amount,
            Type = t.Type,
            TransactionDate = t.TransactionDate
        }).ToList();
    }
}
