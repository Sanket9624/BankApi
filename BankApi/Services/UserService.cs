using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankApi.Entities;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> DepositAsync(int userId, decimal amount, string description)
    {
        var user = await _repository.GetUserByIdAsync(userId);
        if (user?.Account == null) return false;

        return await _repository.DepositAsync(user.Account.AccountId, amount, description);
    }

    public async Task<bool> WithdrawAsync(int userId, decimal amount, string description)
    {
        var user = await _repository.GetUserByIdAsync(userId);
        if (user?.Account == null) return false;

        return await _repository.WithdrawAsync(user.Account.AccountId, amount,description);
    }

    public async Task<bool> TransferAsync(int userId, string receiverAccountNumber, decimal amount, string description)
    {
        var sender = await _repository.GetUserByIdAsync(userId);
        //var receiver = await _repository.GetAccountByIdAsync(userId);
        if (sender?.Account == null) return false;

        return await _repository.TransferAsync(sender.Account.AccountId, receiverAccountNumber, amount,description);
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
            TransactionDate = t.TransactionDate,
            Description = t.Description,
            ReceiverName = t.Type == TransactionType.Transfer && t.ReceiverAccount?.Users != null
                ? $"{t.ReceiverAccount.Users.FirstName} {t.ReceiverAccount.Users.LastName}"
                : null
        }).ToList();
    }

    public async Task<List<TransactionDto>> GetCustomTransactionHistoryAsync(int userId, DateTime? startDate, DateTime? endDate, TransactionType? type)
    {
        var user = await _repository.GetUserByIdAsync(userId);
        if (user?.Account == null) return new List<TransactionDto>();

        endDate ??= DateTime.UtcNow.Date;
        startDate ??= endDate.Value.AddDays(-30);
        if (startDate > endDate)
            (startDate, endDate) = (endDate, startDate);

        var transactions = await _repository.GetCustomeTransactionHistoryAsync(user.Account.AccountId, startDate, endDate, type);

        return transactions.Select(t => new TransactionDto
        {
            TransactionId = t.TransactionId,
            Amount = t.Amount,
            Type = t.Type,
            TransactionDate = t.TransactionDate,
            Description = t.Description,
            ReceiverName = t.Type == TransactionType.Transfer && t.ReceiverAccount?.Users != null
                ? $"{t.ReceiverAccount.Users.FirstName} {t.ReceiverAccount.Users.LastName}"
                : null,
            SenderName = t.Type == TransactionType.Transfer && t.SenderAccount?.Users != null
                ? $"{t.SenderAccount.Users.FirstName} {t.SenderAccount.Users.LastName}"
                : null
        }).ToList();
    }
}
