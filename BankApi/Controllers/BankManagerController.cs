using BankApi.Services;
using BankApi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankApi.Controllers
{
    [ApiController]
    [Route("api/bank-manager")]
    //[Authorize(Policy = "SuperAdminOnly")]
    [Authorize(Policy = "SuperAdminOrBankManager")]
    public class BankManagerController : ControllerBase
    {
        private readonly IBankManagerService _bankManagerService;

        public BankManagerController(IBankManagerService bankManagerService)
        {
            _bankManagerService = bankManagerService;
        }

        // Get total transaction count
        [HttpGet("total-transactions")]
        public async Task<IActionResult> GetTotalTransactionCount()
        {
            var count = await _bankManagerService.GetTotalTransactionCount();
            return Ok(new { TotalTransactions = count });
        }

        // Get all transactions
        [HttpGet("transactions")]
        public async Task<IActionResult> GetAllTransactions()
        {
            var transactions = await _bankManagerService.GetAllTransactions();
            return Ok(transactions);
        }

        // Get deposit transactions
        [HttpGet("transactions/deposit")]
        public async Task<IActionResult> GetDepositTransactions()
        {
            var transactions = await _bankManagerService.GetDepositTransactions();
            return Ok(transactions);
        }

        // Get withdrawal transactions
        [HttpGet("transactions/withdraw")]
        public async Task<IActionResult> GetWithdrawTransactions()
        {
            var transactions = await _bankManagerService.GetWithdrawTransactions();
            return Ok(transactions);
        }

        // Get total deposited amount
        [HttpGet("total-deposited")]
        public async Task<IActionResult> GetTotalAmountDeposited()
        {
            var totalDeposited = await _bankManagerService.GetTotalAmountDeposited();
            return Ok(new { TotalDeposited = totalDeposited });
        }

        // Get total withdrawn amount
        [HttpGet("total-withdrawn")]
        public async Task<IActionResult> GetTotalAmountWithdrawn()
        {
            var totalWithdrawn = await _bankManagerService.GetTotalAmountWithdrawn();
            return Ok(new { TotalWithdrawn = totalWithdrawn });
        }

        // Get total bank balance
        [HttpGet("total-bank-balance")]
        public async Task<IActionResult> GetTotalBankBalance()
        {
            var balance = await _bankManagerService.GetTotalBankBalance();
            return Ok(new { TotalBankBalance = balance });
        }

        // Get all transactions with details
        [HttpGet("transactions/details")]
        public async Task<IActionResult> GetAllTransactionsWithDetails()
        {
            var transactionDetails = await _bankManagerService.GetAllTransactionsWithDetails();
            return Ok(transactionDetails);
        }

        // Get total amount per user
        [HttpGet("transactions/total-per-user")]
        public async Task<IActionResult> GetTotalAmountPerUser()
        {
            var userSummaries = await _bankManagerService.GetTotalAmountPerUser();
            return Ok(userSummaries);
        }

        // Get bank manager overview
        [HttpGet("overview")]
        public async Task<IActionResult> GetBankManagerOverview()
        {
            var overview = await _bankManagerService.GetBankManagerOverview();
            return Ok(overview);
        }

        // Get transaction history for a specific user
        [HttpGet("user-transactions/{userId}")]
        public async Task<IActionResult> GetUserTransactionHistory(int userId)
        {
            var transactionHistory = await _bankManagerService.GetUserTransactionHistory(userId);
            return Ok(transactionHistory);
        }

        // Get all accounts with user details
        [HttpGet("accounts")]
        public async Task<IActionResult> GetAllAccountsWithUserDetails()
        {
            var accounts = await _bankManagerService.GetAllAccountsWithUserDetails();
            return Ok(accounts);
        }
    }
}
