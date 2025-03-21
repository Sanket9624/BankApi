﻿using System;
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


    public async Task<bool> DepositAsync(int accountId, decimal amount ,string description )
    {
        var account = await GetAccountByIdAsync(accountId);
        if (account == null || amount <= 0) return false;

        account.Balance += amount;

        var transaction = new Transactions
        {
            ReceiverAccountId = accountId,
            Type = TransactionType.Deposit,
            Amount = amount,
            Description = description
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> WithdrawAsync(int accountId, decimal amount, string description)
    {
        var account = await GetAccountByIdAsync(accountId);
        if (account == null || amount <= 0 || account.Balance < amount) return false;

        account.Balance -= amount;

        var transaction = new Transactions
        {
            SenderAccountId = accountId,
            Type = TransactionType.Withdraw,
            Amount = amount,
            Description = description
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TransferAsync(int senderAccountId, string ReceiverAccountNumber, decimal amount, string description)
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
            Amount = amount,
            Description = description
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
            .Where(t => t.SenderAccountId == accountId || t.ReceiverAccountId == accountId) // Fetch all transactions
            .Include(t => t.ReceiverAccount)
            .ThenInclude(a => a.Users)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<List<Transactions>> GetCustomeTransactionHistoryAsync(int accountId, DateTime? startDate, DateTime? endDate, TransactionType? type)
    {
        var query = _context.Transactions
            .Where(t => t.SenderAccountId == accountId || t.ReceiverAccountId == accountId);

        // Apply date filter
        if (startDate.HasValue)
            query = query.Where(t => t.TransactionDate.Date >= startDate.Value.Date);

        if (endDate.HasValue)
            query = query.Where(t => t.TransactionDate.Date <= endDate.Value.Date);

        // Apply type filter
        if (type.HasValue)
            query = query.Where(t => t.Type == type.Value);

        return await query
            .Include(t => t.ReceiverAccount)
            .ThenInclude(a => a.Users)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }
}

