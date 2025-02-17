using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankApi.Data;
using BankApi.Entities;
using Microsoft.EntityFrameworkCore;

public class BankingRepository : IBankingRepository
{
    private readonly BankDb1Context _context;

    public BankingRepository(BankDb1Context context)
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


    public async Task<bool> DepositAsync(int accountId, decimal amount)
    {
        var account = await GetAccountByIdAsync(accountId);
        if (account == null || amount <= 0) return false;

        account.Balance += amount;

        var transaction = new Transactions
        {
            ReceiverAccountId = accountId,
            Type = TransactionType.Deposit,
            Amount = amount
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> WithdrawAsync(int accountId, decimal amount)
    {
        var account = await GetAccountByIdAsync(accountId);
        if (account == null || amount <= 0 || account.Balance < amount) return false;

        account.Balance -= amount;

        var transaction = new Transactions
        {
            SenderAccountId = accountId,
            Type = TransactionType.Withdraw,
            Amount = amount
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TransferAsync(int senderAccountId, string ReceiverAccountNumber, decimal amount)
    {
        var sender = await GetAccountByIdAsync(senderAccountId);
        var receiver = await GetAccountByNumberAsync(ReceiverAccountNumber);


        if (sender == null || receiver == null || amount <= 0 || sender.Balance < amount) return false;

        sender.Balance -= amount;
        receiver.Balance += amount;

        var transaction = new Transactions
        {
            SenderAccountId = senderAccountId,
            ReceiverAccountId = receiver.AccountId,
            Type = TransactionType.Transfer,
            Amount = amount
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
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }
}
