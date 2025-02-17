using BankApi.Dto;
using BankApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BankApi.Controllers
{
    [Authorize(Policy = "SuperAdminOnly")]
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("superadmin")]
        public async Task<IActionResult> GetAdmin()
        {
            var admin = await _adminService.GetAdminAsync();
            if (admin == null) return NotFound(new { Message = "SuperAdmin not found." });
            return Ok(admin);
        }

        [HttpPost("roles")]
        public async Task<IActionResult> CreateRole([FromBody] RoleRequestDto roleRequestDto)
        {
            if (string.IsNullOrWhiteSpace(roleRequestDto.RoleName))
                return BadRequest(new { Message = "Role name cannot be empty." });

            var role = await _adminService.CreateRoleAsync(roleRequestDto.RoleName);
            return CreatedAtAction(nameof(GetRoles), new { roleName = role.RoleName }, role);
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _adminService.GetRolesAsync();
            return Ok(roles);
        }

        [HttpDelete("roles/{roleId}")]
        public async Task<IActionResult> DeleteRole(int roleId)
        {
            var result = await _adminService.DeleteRoleAsync(roleId);
            if (!result) return NotFound(new { Message = "Role not found." });

            return Ok($"RoleId {roleId} Deleted Succesfully");
        }

        [HttpPost("bank-managers")]
        public async Task<IActionResult> CreateBankManager([FromBody] BankManagerDto bankManagerDto)
        {
            if (bankManagerDto == null) return BadRequest(new { Message = "Invalid bank manager data." });

            var createdManager = await _adminService.CreateBankManagerAsync(bankManagerDto);
            return Ok($"Manager Created Succesfully. UserName : {bankManagerDto.Email}");
        }

        [HttpPut("bank-managers/{userId}")]
        public async Task<IActionResult> UpdateBankManager(int userId , [FromBody] BankManagerDto bankManagerDto)
        {
            if (bankManagerDto == null) return BadRequest(new { Message = "Invalid data." });

            var updatedManager = await _adminService.UpdateBankManagerAsync(userId , bankManagerDto);
            if (updatedManager == null) return NotFound(new { Message = "Bank Manager not found." });

            return Ok($"Manager details Updated Succesfully. UserName : {updatedManager}");
        }

        [HttpGet("bank-managers")]
        public async Task<IActionResult> GetBankManagers()
        {
            var managers = await _adminService.GetBankManagersAsync();
            return Ok(managers);
        }

        [HttpGet("users")]
         public async Task<IActionResult> GetAllUsersExceptAdmin()
            {
                var users = await _adminService.GetAllUsersExceptAdminAsync();
                return Ok(users);
            }

        [HttpPut("users/{userId}")]
        public async Task<IActionResult> UpdateUser(int userId ,[FromBody] UserRequestDto userRequestDto)
        {
            if (userRequestDto == null) return BadRequest(new { Message = "Invalid data." });

            var updatedUser = await _adminService.UpdateUserAsync(userId, userRequestDto);
            if (updatedUser == null) return NotFound(new { Message = "User not found." });

            return Ok(updatedUser);
        }

        [HttpDelete("users/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var result = await _adminService.DeleteUserAsync(userId);
            if (!result) return NotFound(new { Message = "User not found." });

            return Ok($"User Deleted Succesfully");
        }
    }
}
