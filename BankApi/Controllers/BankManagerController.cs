using BankApi.Dto;
using BankApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "SuperAdminOrBankManager")]
    [ApiController]
    public class BankManagerController : ControllerBase
    {
        private readonly IBankManagerService _bankManagerService;

        public BankManagerController(IBankManagerService bankManagerService)
        {
            _bankManagerService = bankManagerService;
        }

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

      [HttpGet("total-accounts")]
        public async Task<IActionResult> GetTotalAccounts()
        {
            var result = await _bankManagerService.GetTotalAccounts();
            return Ok(result);
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions(
        [FromQuery] int? userId = null,
        [FromQuery] string? transactionType = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
        {
            var transactions = await _bankManagerService.GetTransactions(userId, transactionType, startDate, endDate);
            return Ok(transactions);
        }

    }
}
