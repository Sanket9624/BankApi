using BankApi.Dto;
using BankApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankingManagementSystem.Controllers
{
    [Route("api/[controller]")]
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

        //[HttpGet("all")]
        //public async Task<IActionResult> GetAllTransactions()
        //{
        //    try
        //    {
        //        var transactions = await _bankManagerService.GetAllTransactions();
        //        return Ok(transactions);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Failed to retrieve transactions: {ex.Message}");
        //    }
        //}

        //[HttpGet("type/{transactionType}")]
        //public async Task<IActionResult> GetTransactionsByType(string transactionType)
        //{
        //    try
        //    {
        //        var transactions = await _bankManagerService.GetTransactionsByType(transactionType);
        //        if (!transactions.Any())
        //            return NotFound($"No transactions found for type '{transactionType}'.");

        //        return Ok(transactions);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Failed to retrieve transactions: {ex.Message}");
        //    }
        //}

        //[HttpGet("type/{transactionType}/filter")]
        //public async Task<IActionResult> GetTransactionsByTypeAndDateRange(string transactionType, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        //{
        //    try
        //    {
        //        var transactions = await _bankManagerService.GetTransactionsByTypeAndDateRange(transactionType, startDate, endDate);
        //        if (!transactions.Any())
        //            return NotFound($"No transactions found for type '{transactionType}' in the specified date range.");

        //        return Ok(transactions);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Failed to retrieve filtered transactions: {ex.Message}");
        //    }
        //}


        //[HttpGet("date-range")]
        //public async Task<IActionResult> GetTransactionsByDateRange(DateTime startDate, DateTime endDate)
        //{
        //    try
        //    {
        //        if (startDate > endDate)
        //            return BadRequest("Start date must be before end date.");

        //        var transactions = await _bankManagerService.GetTransactionsByDateRange(startDate, endDate);
        //        if (!transactions.Any())
        //            return NotFound("No transactions found for the specified date range.");

        //        return Ok(transactions);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Failed to retrieve transactions: {ex.Message}");
        //    }
        //}

        //[HttpGet("user-transactions")]
        //public async Task<IActionResult> GetUserTransactions(int? userId, string? email)
        //{
        //    try
        //    {
        //        var transactions = await _bankManagerService.GetUserTransactions(userId, email);
        //        if (!transactions.Any())
        //            return NotFound("No user transactions found.");

        //        return Ok(transactions);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Failed to retrieve user transactions: {ex.Message}");
        //    }
        //}

        //[HttpGet("user-date-range")]
        //public async Task<IActionResult> GetUserTransactionsByDateRange(int userId, DateTime startDate, DateTime endDate)
        //{
        //    try
        //    {
        //        if (startDate > endDate)
        //            return BadRequest("Start date must be before end date.");

        //        var transactions = await _bankManagerService.GetUserTransactionsByDateRange(userId, startDate, endDate);
        //        if (!transactions.Any())
        //            return NotFound("No transactions found for the specified user and date range.");

        //        return Ok(transactions);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Failed to retrieve user transactions: {ex.Message}");
        //    }
        //}

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
