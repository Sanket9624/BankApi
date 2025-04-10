﻿using BankApi.Attributes;
using BankApi.Dto;
using BankApi.Enums;
using BankApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using BankApi.Entities;

[Authorize(Policy = "CustomerOnly")]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _bankingService;

    public UserController(IUserService bankingService)
    {
        _bankingService = bankingService ?? throw new ArgumentNullException(nameof(bankingService));
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

    [HttpPost("deposit")]
    [PermissionAuthorize(nameof(PermissionEnum.MakeDeposit))]
    public async Task<IActionResult> Deposit([FromBody] AmountRequestDto request)
    {
        var userId = GetUserId();
        var result = await _bankingService.DepositAsync(userId, request.Amount, request.Description);

        if (!result)
            return BadRequest(new { Message = "Deposit failed. Please check your account details." });

        return Ok(new { Message = "Deposit successful" });
    }

    [HttpPost("withdraw")]
    [PermissionAuthorize(nameof(PermissionEnum.MakeWithdrawal))]
    public async Task<IActionResult> Withdraw([FromBody] AmountRequestDto request)
    {
        var userId = GetUserId();
        var result = await _bankingService.WithdrawAsync(userId, request.Amount, request.Description);

        if (!result)
            return BadRequest(new { Message = "Withdrawal failed. Insufficient balance or invalid request." });

        return Ok(new { Message = "Withdrawal successful" });
    }

    [HttpPost("transfer")]
    [PermissionAuthorize(nameof(PermissionEnum.MakeTransfer))]
    public async Task<IActionResult> Transfer([FromBody] TransferRequestDto request)
    {
        var userId = GetUserId();
        var result = await _bankingService.TransferAsync(userId, request.ReceiverAccountNumber, request.Amount, request.Description);

        if (!result)
            return BadRequest(new { Message = "Transfer failed. Check recipient details or balance." });

        return Ok(new { Message = "Transfer successful" });
    }

    [HttpGet("balance")]
    [PermissionAuthorize(nameof(PermissionEnum.ViewBalance))]
    public async Task<IActionResult> GetBalance()
    {
        var userId = GetUserId();
        var balanceDto = await _bankingService.GetBalanceAsync(userId);
        return Ok(balanceDto);
    }

    [HttpGet("transactions")]
    [PermissionAuthorize(nameof(PermissionEnum.ViewTransactions))]
    public async Task<IActionResult> GetTransactionHistory()
    {
        var userId = GetUserId();
        var transactions = await _bankingService.GetTransactionHistoryAsync(userId);
        return Ok(transactions);
    }

    [HttpGet("customTransactions")]
    [PermissionAuthorize(nameof(PermissionEnum.ViewCustomTransactions))]
    public async Task<IActionResult> GetCustomTransactionHistory(DateTime? startDate, DateTime? endDate, TransactionType? type, TransactionStatus? status)
    {
        var userId = GetUserId();
        var transactions = await _bankingService.GetCustomTransactionHistoryAsync(userId, startDate, endDate, type, status);
        return Ok(transactions);
    }
}