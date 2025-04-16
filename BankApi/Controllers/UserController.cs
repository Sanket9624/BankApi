using BankApi.Dto;
using BankApi.Entities;
using BankApi.Enums;
using BankApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _bankingService;

    public UserController(IUserService bankingService)
    {
        _bankingService = bankingService;
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

    [HasPermission(Permissions.MakeDeposit)]
    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit([FromBody] AmountRequestDto request)
    {
        var userId = GetUserId();
        var result = await _bankingService.DepositAsync(userId, request.Amount, request.Description);

        if (!result)
            return BadRequest(new { Message = "Deposit failed. Please check your account details." });

        return Ok(new { Message = "Deposit successful" });
    }

    [HasPermission(Permissions.MakeWithdrawal)]
    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw([FromBody] AmountRequestDto request)
    {
        var userId = GetUserId();
        var result = await _bankingService.WithdrawAsync(userId, request.Amount, request.Description);

        if (!result)
            return BadRequest(new { Message = "Withdrawal failed. Insufficient balance or invalid request." });

        return Ok(new { Message = "Withdrawal successful" });
    }

    [HasPermission(Permissions.MakeTransfer)]
    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] TransferRequestDto request)
    {
        var userId = GetUserId();
        var result = await _bankingService.TransferAsync(userId, request.ReceiverAccountNumber, request.Amount, request.Description);

        if (!result)
            return BadRequest(new { Message = "Transfer failed. Check recipient details or balance." });

        return Ok(new { Message = "Transfer successful" });
    }

    [HasPermission(Permissions.ViewBalance)]
    [HttpGet("balance")]
    public async Task<IActionResult> GetBalance()
    {
        var userId = GetUserId();
        var balanceDto = await _bankingService.GetBalanceAsync(userId);
        return Ok(balanceDto);
    }

    [HasPermission(Permissions.ViewTransactions)]
    [HttpGet("transactions")]
    public async Task<IActionResult> GetCustomTransactionHistory(DateTime? startDate, DateTime? endDate, TransactionType? type, TransactionStatus? status)
    {
        var userId = GetUserId();
        var transactions = await _bankingService.GetCustomTransactionHistoryAsync(userId, startDate, endDate, type, status);
        return Ok(transactions);
    }
}
