using BankApi.Attributes;
using BankApi.Dto;
using BankApi.Dto.Request;
using BankApi.Dto.Response;
using BankApi.Entities;
using BankApi.Enums;
using BankApi.Services.Interfaces;
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

        [HttpGet("roles")]
        [PermissionAuthorize(nameof(PermissionEnum.ViewRoles))]
        public async Task<ActionResult<IEnumerable<RoleResponseDto>>> GetRoles()
        {
            var roles = await _adminService.GetRolesAsync();
            return Ok(roles);
        }

        [HttpPost("roles")]
        [PermissionAuthorize(nameof(PermissionEnum.CreateRole))]
        public async Task<ActionResult<RoleResponseDto>> CreateRole([FromBody] RoleRequestDto roleDto)
        {
            var creatorRoleId = int.Parse(User.FindFirst("RoleId")?.Value ?? "0");
            var createdRole = await _adminService.CreateRoleAsync(roleDto.RoleName, creatorRoleId);
            return CreatedAtAction(nameof(GetRoles), new { roleId = createdRole.RoleId }, createdRole);
        }

        [HttpDelete("roles/{roleId}")]
        [PermissionAuthorize(nameof(PermissionEnum.DeleteRole))]
        public async Task<IActionResult> DeleteRole(int roleId)
        {
            var result = await _adminService.DeleteRoleAsync(roleId);
            if (!result) return BadRequest("Cannot delete this role.");
            return NoContent();
        }

        [HttpGet("managers")]
        [PermissionAuthorize(nameof(PermissionEnum.ViewUsers))]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetBankManagers()
        {
            var managers = await _adminService.GetBankManagersAsync();
            return Ok(managers);
        }

        [HttpPost("managers")]
        [PermissionAuthorize(nameof(PermissionEnum.CreateManager))]
        public async Task<ActionResult<UserResponseDto>> CreateBankManager([FromBody] BankManagerDto managerDto)
        {
            var createdManager = await _adminService.CreateBankManagerAsync(managerDto);
            return CreatedAtAction(nameof(GetBankManagers), createdManager);
        }

        [HttpPost("verify-otp")]
        [PermissionAuthorize(nameof(PermissionEnum.VerifyManager))]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerificationDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Otp))
                return BadRequest(new { Message = "Invalid OTP data." });

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

        [HttpPost("approve-account")]
        [PermissionAuthorize(nameof(PermissionEnum.ApproveAccount))]
        public async Task<IActionResult> ApproveAccount([FromBody] ApproveAccountDto request, AccountType accountType, string? rejectedReason)
        {
            var result = await _adminService.ApproveAccountRequest(request.UserId, request.IsApproved, accountType, rejectedReason);
            return Ok(result);
        }

        [HttpGet("users-status")]
        [PermissionAuthorize(nameof(PermissionEnum.ViewUsers))]
        public async Task<IActionResult> GetUsersWithStatus()
        {
            var users = await _adminService.GetAllUsersWithStatusAsync();
            return Ok(users);
        }

        [HttpGet("users")]
        [PermissionAuthorize(nameof(PermissionEnum.ViewUsers))]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAllUsersExceptAdmin()
        {
            var users = await _adminService.GetAllUsersExceptAdminAsync();
            return Ok(users);
        }

        [HttpPut("users/{userId}")]
        [PermissionAuthorize(nameof(PermissionEnum.UpdateUser))]
        public async Task<ActionResult<UserResponseDto>> UpdateUser(int userId, [FromBody] BankMangerUpdateDto dto)
        {
            var updatedUser = await _adminService.UpdateUserAsync(userId, dto);
            if (updatedUser == null) return NotFound("User not found or cannot be updated.");
            return Ok(updatedUser);
        }

        [HttpDelete("users/{userId}")]
        [PermissionAuthorize(nameof(PermissionEnum.DeleteUser))]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var result = await _adminService.DeleteUserAsync(userId);
            if (!result) return NotFound("User not found or cannot be deleted.");
            return NoContent();
        }

        [HttpGet("pending-transactions")]
        [PermissionAuthorize(nameof(PermissionEnum.ViewPendingTransactions))]
        public async Task<ActionResult<List<TransactionResponseDto>>> GetPendingTransactions()
        {
            var transactions = await _adminService.GetPendingTransactionsAsync();
            return Ok(transactions);
        }

        [HttpPost("approve/{transactionId}")]
        [PermissionAuthorize(nameof(PermissionEnum.ApproveTransaction))]
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

        [HttpPost("reject/{transactionId}")]
        [PermissionAuthorize(nameof(PermissionEnum.RejectTransaction))]
        public async Task<IActionResult> RejectTransaction(int transactionId, [FromBody] string reason)
        {
            var result = await _adminService.RejectTransactionAsync(transactionId, reason);
            if (!result) return BadRequest("Transaction rejection failed.");
            return Ok("Transaction rejected successfully.");
        }
    }
}
