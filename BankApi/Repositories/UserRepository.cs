using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankApi.Data;
using BankApi.Entities;
using Microsoft.EntityFrameworkCore;

public class UserRepository : IUserRepository
{
    private readonly BankDb1Context _context;

    public UserRepository(BankDb1Context context)
    {
        _context = context;
    }

    public async Task<Account> GetAccountByIdAsync(int accountId)
    {
        return await _context.Account.FirstOrDefaultAsync(a => a.AccountId == accountId);
    }

    public async Task<Users> GetUserByIdAsync(int userId)
    {
        return await _context.Users.Include(u => u.Account).FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<Account> GetAccountByNumberAsync(string accountNumber)
    {
        return await _context.Account.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
    }

    public async Task<bool> RequestDepositAsync(int accountId, decimal amount, string description, TransactionStatus status)
    {
        if (amount <= 0) return false;

        var account = await GetAccountByIdAsync(accountId);
        if (account == null) return false;

        var transaction = new Transactions
        {
            ReceiverAccountId = accountId,
            Type = TransactionType.Deposit,
            Amount = amount,
            Description = description,
            Status = status
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RequestWithdrawAsync(int accountId, decimal amount, string description, TransactionStatus status)
    {
        if (amount <= 0) return false;

        var account = await GetAccountByIdAsync(accountId);
        if (account == null || account.Balance < amount) return false;

        var transaction = new Transactions
        {
            SenderAccountId = accountId,
            Type = TransactionType.Withdraw,
            Amount = amount,
            Description = description,
            Status = status
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RequestTransferAsync(int senderAccountId, string receiverAccountNumber, decimal amount, string description, TransactionStatus status)
    {
        if (amount <= 0) return false;

        var sender = await GetAccountByIdAsync(senderAccountId);
        var receiver = await GetAccountByNumberAsync(receiverAccountNumber);

        if (sender == null || receiver == null || sender.Balance < amount) return false;

        var transaction = new Transactions
        {
            SenderAccountId = senderAccountId,
            ReceiverAccountId = receiver.AccountId,
            Type = TransactionType.Transfer,
            Amount = amount,
            Description = description,
            Status = status
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        return true;
    }


    public async Task<decimal> GetBalanceAsync(int accountId)
    {
        var account = await GetAccountByIdAsync(accountId);
        return account?.Balance ?? 0;
    }

    public async Task<List<Transactions>> GetTransactionHistoryAsync(int accountId)
    {
        return await _context.Transactions
            .Where(t => t.SenderAccountId == accountId || t.ReceiverAccountId == accountId)
            .Include(t => t.ReceiverAccount.Users)
            .Include(t => t.SenderAccount.Users)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<List<Transactions>> GetCustomeTransactionHistoryAsync(int accountId, DateTime? startDate, DateTime? endDate, TransactionType? type, TransactionStatus? status)
    {
        var query = _context.Transactions.AsQueryable();

        query = query.Where(t => t.SenderAccountId == accountId || t.ReceiverAccountId == accountId);

        if (startDate.HasValue)
            query = query.Where(t => t.TransactionDate.Date >= startDate.Value.Date);

        if (endDate.HasValue)
            query = query.Where(t => t.TransactionDate.Date <= endDate.Value.Date);

        if (type.HasValue)
            query = query.Where(t => t.Type == type.Value);

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        return await query
            .Include(t => t.ReceiverAccount.Users)
            .Include(t => t.SenderAccount.Users)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }
}
