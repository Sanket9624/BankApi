using BankApi.Entities;
using BankApi.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

[Authorize(Policy = "CustomerOnly")]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _bankingService;

    public UserController(IUserService bankingService)
    {
        _bankingService = bankingService;
    }

    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit([FromBody] AmountRequestDto request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

        var result = await _bankingService.DepositAsync(userId, request.Amount,request.Description);
        return result ? Ok(new { Message = "Deposit successful" }) : BadRequest(new { Message = "Deposit failed" });
    }

    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw([FromBody] AmountRequestDto request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

        var result = await _bankingService.WithdrawAsync(userId, request.Amount,request.Description);
        return result ? Ok(new { Message = "Withdrawal successful" }) : BadRequest(new { Message = "Insufficient balance" });
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] TransferRequestDto request)
    {
        var senderUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

        var result = await _bankingService.TransferAsync(senderUserId, request.ReceiverAccountNumber, request.Amount,request.Description);
        return result ? Ok(new { Message = "Transfer successful" }) : BadRequest(new { Message = "Transfer failed" });
    }

    [HttpGet("balance")]
    public async Task<IActionResult> GetBalance()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

        var balanceDto = await _bankingService.GetBalanceAsync(userId);
        return Ok(balanceDto);
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactionHistory()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var transactions = await _bankingService.GetTransactionHistoryAsync(userId);
        return Ok(transactions);
    }

    [HttpGet("customTransactions")]
    public async Task<IActionResult> GetCustomTransactionHistory(DateTime? startDate, DateTime? endDate, TransactionType? type)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var transactions = await _bankingService.GetCustomTransactionHistoryAsync(userId, startDate, endDate, type);
        return Ok(transactions);
    }


}
