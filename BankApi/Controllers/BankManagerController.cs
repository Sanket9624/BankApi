using BankApi.Dto;
using BankApi.Enums;
using BankApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class BankManagerController : ControllerBase
    {
        private readonly IBankManagerService _bankManagerService;

        public BankManagerController(IBankManagerService bankManagerService)
        {
            _bankManagerService = bankManagerService;
        }

        [HasPermission(Permissions.BankSummary)]
        [HttpGet("summary")]
        public async Task<IActionResult> GetBankSummary()
        {
            try
            {
                var summary = await _bankManagerService.GetAllTimeBankBalanceSheet();
                if (summary == null)
                    return NotFound("Bank summary not available.");

                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while fetching the bank summary: {ex.Message}");
            }
        }

        [HasPermission(Permissions.ViewUsers)]
        [HttpGet("user-account/{userId}")]
        public async Task<IActionResult> GetUserAccountDetails(int userId)
        {
            try
            {
                var userAccount = await _bankManagerService.GetUserAccountDetails(userId);
                if (userAccount == null)
                    return NotFound($"No account found for user ID {userId}.");

                return Ok(userAccount);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to retrieve user account details: {ex.Message}");
            }
        }

        [HasPermission(Permissions.ViewUsers)]
        [HttpGet("account/{accountNumber}")]
        public async Task<IActionResult> GetUserDetailsByAccountNumber(string accountNumber)
        {
            try
            {
                var user = await _bankManagerService.GetUserDetailsByAccountNumber(accountNumber);
                if (user == null)
                    return NotFound($"No user found with account number '{accountNumber}'.");

                return Ok(user);
            }
            catch (Exception ex)
            { 
                return StatusCode(500, $"Failed to retrieve user details: {ex.Message}");
            }
        }

        [HasPermission(Permissions.ViewUsers)]
        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetUserDetailsByEmail(string email)
        {
            try
            {
                var user = await _bankManagerService.GetUserDetailsByEmail(email);
                if (user == null)
                    return NotFound($"No user found with email '{email}'.");

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to retrieve user details: {ex.Message}");
            }
        }

        [HasPermission(Permissions.ViewUsers)]
        [HttpGet("total-accounts")]
        public async Task<IActionResult> GetTotalAccounts()
        {
            var result = await _bankManagerService.GetTotalAccounts();
            return Ok(result);
        }

        [HasPermission(Permissions.GetTransactions)]
        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions(
            [FromQuery] int? userId,
            [FromQuery] string? transactionType,
            [FromQuery] BankApi.Entities.TransactionStatus? status,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var transactions = await _bankManagerService.GetTransactions(userId, transactionType, status, startDate, endDate);
            return Ok(transactions);
        }
    }
}
