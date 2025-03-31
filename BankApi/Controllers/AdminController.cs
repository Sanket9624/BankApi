using BankApi.Dto;
using BankApi.Dto.Request;
using BankApi.Dto.Response;
using BankApi.Entities;
using BankApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankApi.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService ?? throw new ArgumentNullException(nameof(adminService));
        }

        [Authorize(Policy = "SuperAdminOnly")]
        [HttpGet("roles")]
        public async Task<ActionResult<IEnumerable<RoleResponseDto>>> GetRoles()
        {
            var roles = await _adminService.GetRolesAsync();
            return Ok(roles);
        }

        [Authorize(Policy = "SuperAdminOnly")]
        [HttpPost("roles")]
        public async Task<ActionResult<RoleResponseDto>> CreateRole([FromBody] RoleRequestDto roleDto)
        {
            var createdRole = await _adminService.CreateRoleAsync(roleDto.RoleName);
            return CreatedAtAction(nameof(GetRoles), createdRole);
        }

        [Authorize(Policy = "SuperAdminOnly")]
        [HttpDelete("roles/{roleId}")]
        public async Task<IActionResult> DeleteRole(int roleId)
        {
            var result = await _adminService.DeleteRoleAsync(roleId);
            if (!result) return BadRequest("Cannot delete this role.");
            return NoContent();
        }

        [Authorize(Policy = "SuperAdminOnly")]
        [HttpGet("managers")]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetBankManagers()
        {
            var managers = await _adminService.GetBankManagersAsync();
            return Ok(managers);
        }

        [Authorize(Policy = "SuperAdminOnly")]
        [HttpPost("managers")]
        public async Task<ActionResult<UserResponseDto>> CreateBankManager([FromBody] BankManagerDto managerDto)
        {
            var createdManager = await _adminService.CreateBankManagerAsync(managerDto);
            return CreatedAtAction(nameof(GetBankManagers), createdManager);
        }

        [Authorize(Policy = "SuperAdminOnly")]
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerificationDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Otp))
            {
                return BadRequest(new { Message = "Invalid OTP data." });
            }

            try
            {
                var result = await _adminService.VerifyOtpAsync(request.Email, request.Otp);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [Authorize(Policy = "SuperAdminOrBankManager")]
        [HttpPost("approve-account")]
        public async Task<IActionResult> ApproveAccount([FromBody] ApproveAccountDto request, AccountType accountType, string? rejectedReason)
        {
            var result = await _adminService.ApproveAccountRequest(request.UserId, request.IsApproved, accountType, rejectedReason);
            return Ok(result);
        }

        [Authorize(Policy = "SuperAdminOrBankManager")]
        [HttpGet("users-status")]
        public async Task<IActionResult> GetUsersWithStatus()
        {
            var users = await _adminService.GetAllUsersWithStatusAsync();
            return Ok(users);
        }

        [Authorize(Policy = "SuperAdminOrBankManager")]
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAllUsersExceptAdmin()
        {
            var users = await _adminService.GetAllUsersExceptAdminAsync();
            return Ok(users);
        }

        [Authorize(Policy = "SuperAdminOnly")]
        [HttpPut("users/{userId}")]
        public async Task<ActionResult<UserResponseDto>> UpdateUser(int userId, [FromBody] BankMangerUpdateDto dto)
        {
            var updatedUser = await _adminService.UpdateUserAsync(userId, dto);
            if (updatedUser == null) return NotFound("User not found or cannot be updated.");
            return Ok(updatedUser);
        }

        [Authorize(Policy = "SuperAdminOnly")]
        [HttpDelete("users/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var result = await _adminService.DeleteUserAsync(userId);
            if (!result) return NotFound("User not found or cannot be deleted.");
            return NoContent();
        }

        [Authorize(Policy = "SuperAdminOrBankManager")]
        [HttpGet("approved-accounts")]
        public async Task<IActionResult> GetApprovedAccounts()
        {
            var accounts = await _adminService.GetApprovedAccountsAsync();
            return Ok(accounts);
        }

        [Authorize(Policy = "SuperAdminOrBankManager")]
        [HttpGet("pending-transactions")]
        public async Task<ActionResult<List<TransactionResponseDto>>> GetPendingTransactions()
        {
            var transactions = await _adminService.GetPendingTransactionsAsync();
            return Ok(transactions);
        }

        [Authorize(Policy = "SuperAdminOrBankManager")]
        [HttpPost("approve/{transactionId}")]
        public async Task<IActionResult> ApproveTransaction(int transactionId)
        {
            var result = await _adminService.ApproveTransactionAsync(transactionId);
            if (!result.success)
            {
                Console.WriteLine($"Transaction {transactionId} approval failed: {result.errorMessage}");
                return BadRequest(result.errorMessage);
            }
            return Ok("Transaction approved successfully.");
        }

        [Authorize(Policy = "SuperAdminOrBankManager")]
        [HttpPost("reject/{transactionId}")]
        public async Task<IActionResult> RejectTransaction(int transactionId, [FromBody] string reason)
        {
            var result = await _adminService.RejectTransactionAsync(transactionId, reason);
            if (!result) return BadRequest("Transaction rejection failed.");
            return Ok("Transaction rejected successfully.");
        }
    }
}
