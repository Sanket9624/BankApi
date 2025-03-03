using BankApi.Dto;
using BankApi.Dto.Request;
using BankApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BankApi.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [Authorize(Policy = "SuperAdminOnly")]
        [HttpGet("superadmin")]
        public async Task<IActionResult> GetAdmin()
        {
            var admin = await _adminService.GetAdminAsync();
            if (admin == null) return NotFound(new { Message = "SuperAdmin not found." });
            return Ok(admin);
        }

        [Authorize(Policy = "SuperAdminOnly")]
        [HttpPost("roles")]
        public async Task<IActionResult> CreateRole([FromBody] RoleRequestDto roleRequestDto)
        {
            if (string.IsNullOrWhiteSpace(roleRequestDto.RoleName))
                return BadRequest(new { Message = "Role name cannot be empty." });

            var role = await _adminService.CreateRoleAsync(roleRequestDto.RoleName);
            return CreatedAtAction(nameof(GetRoles), new { roleName = role.RoleName }, role);
        }

        [Authorize(Policy = "SuperAdminOnly")]
        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _adminService.GetRolesAsync();
            return Ok(roles);
        }

        [Authorize(Policy = "SuperAdminOnly")]
        [HttpDelete("roles/{roleId}")]
        public async Task<IActionResult> DeleteRole(int roleId)
        {
            var result = await _adminService.DeleteRoleAsync(roleId);
            if (!result) return NotFound(new { Message = "Role not found." });

            return Ok($"RoleId {roleId} Deleted Succesfully");
        }

        [Authorize(Policy = "SuperAdminOnly")]
        [HttpPost("bank-managers")]
        public async Task<IActionResult> CreateBankManager([FromBody] BankManagerDto bankManagerDto)
        {
            if (bankManagerDto == null) return BadRequest(new { Message = "Invalid bank manager data." });

            var createdManager = await _adminService.CreateBankManagerAsync(bankManagerDto);
            return Ok($"Manager Created Succesfully. UserName : {bankManagerDto.Email}");
        }

        [Authorize(Policy = "SuperAdminOnly")]
        [HttpGet("bank-managers")]
        public async Task<IActionResult> GetBankManagers()
        {
            var managers = await _adminService.GetBankManagersAsync();
            return Ok(managers);
        }

        [Authorize(Policy = "SuperAdminOrBankManager")]
        [HttpGet("users")]
         public async Task<IActionResult> GetAllUsersExceptAdmin()
            {
                var users = await _adminService.GetAllUsersExceptAdminAsync();
                return Ok(users);
            }

        [Authorize(Policy = "SuperAdminOnly")]
        [HttpPut("users/{userId}")]
        public async Task<IActionResult> UpdateUser(int userId ,[FromBody] BankMangerUpdateDto bankMangerUpdateDto)
        {
            if (bankMangerUpdateDto == null) return BadRequest(new { Message = "Invalid data." });

            var updatedUser = await _adminService.UpdateUserAsync(userId, bankMangerUpdateDto);
            if (updatedUser == null) return NotFound(new { Message = "User not found." });

            return Ok(updatedUser);
        }

        [Authorize(Policy = "SuperAdminOnly")]
        [HttpDelete("users/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var result = await _adminService.DeleteUserAsync(userId);
            if (!result) return NotFound(new { Message = "User not found." });

            return Ok($"User Deleted Succesfully");
        }
    }
}
