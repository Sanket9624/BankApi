using BankApi.Dto;
using BankApi.Dto.Request;
using BankApi.Enums;
using BankApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
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

        [HasPermission(Permissions.ViewRoles)]
        [HttpGet("roles")]
        public async Task<ActionResult<IEnumerable<RoleResponseDto>>> GetRoles()
        {
            var roles = await _adminService.GetRolesAsync();
            return Ok(roles);
        }

        [HasPermission(Permissions.CreateRole)] 
        [HttpPost("roles")]
        public async Task<ActionResult<RoleResponseDto>> CreateRole([FromBody] RoleRequestDto roleDto)
        {
            var createdRole = await _adminService.CreateRoleAsync(roleDto.RoleName);
            return CreatedAtAction(nameof(GetRoles), createdRole);
        }

        [HasPermission(Permissions.DeleteRole)]
        [HttpDelete("roles/{roleId}")]
        public async Task<IActionResult> DeleteRole(int roleId)
        {
            var result = await _adminService.DeleteRoleAsync(roleId);
            if (!result) return BadRequest("Cannot delete this role.");
            return NoContent();
        }

        [HasPermission(Permissions.ViewUsers)]
        [HttpGet("managers")]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetBankManagers()
        {
            var managers = await _adminService.GetBankManagersAsync();
            return Ok(managers);
        }

        [HasPermission(Permissions.CreateManager)]
        [HttpPost("managers")]
        public async Task<ActionResult<UserResponseDto>> CreateBankManager([FromBody] BankManagerDto managerDto)
        {
            var createdManager = await _adminService.CreateBankManagerAsync(managerDto);
            return CreatedAtAction(nameof(GetBankManagers), createdManager);
        }

        [HttpPost("verify-otp")]
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

        [HasPermission(Permissions.ApproveAccount)]
        [HttpPost("approve-account")]
        public async Task<IActionResult> ApproveAccount([FromBody] ApproveAccountDto request)
        {
            var result = await _adminService.ApproveAccountRequest(request.UserId, request.IsApproved, request.AccountType, request.RejectedReason);
            return Ok(result);
        }

        [HasPermission(Permissions.ViewUsersWithStatus)]
        [HttpGet("users-status")]
        public async Task<IActionResult> GetUsersWithStatus()
        {
            var users = await _adminService.GetAllUsersWithStatusAsync();
            return Ok(users);
        }

        [HasPermission(Permissions.ViewUsers)]
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAllUsersExceptAdmin()
        {
            var users = await _adminService.GetAllUsersExceptAdminAsync();
            return Ok(users);
        }

        [HasPermission(Permissions.UpdateUser)]
        [HttpPut("users/{userId}")]
        public async Task<ActionResult<UserResponseDto>> UpdateUser(int userId, [FromBody] BankMangerUpdateDto dto)
        {
            var updatedUser = await _adminService.UpdateUserAsync(userId, dto);
            if (updatedUser == null) return NotFound("User not found or cannot be updated.");
            return Ok(updatedUser);
        }

        [HasPermission(Permissions.DeleteUser)]
        [HttpDelete("users/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var result = await _adminService.DeleteUserAsync(userId);
            if (!result) return NotFound("User not found or cannot be deleted.");
            return NoContent();
        }


        [HasPermission(Permissions.ViewPendingTransactions)]
        [HttpGet("pending-transactions")]
        public async Task<ActionResult<List<TransactionResponseDto>>> GetPendingTransactions()
        {
            var transactions = await _adminService.GetPendingTransactionsAsync();
            return Ok(transactions);
        }

        [HasPermission(Permissions.ApproveTransaction)]
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

        [HasPermission(Permissions.RejectTransaction)]
        [HttpPost("reject/{transactionId}")]
        public async Task<IActionResult> RejectTransaction(int transactionId, [FromBody] string reason)
        {
            var result = await _adminService.RejectTransactionAsync(transactionId, reason);
            if (!result) return BadRequest("Transaction rejection failed.");
            return Ok("Transaction rejected successfully.");
        }
    }
}
