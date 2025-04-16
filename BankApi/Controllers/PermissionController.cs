using BankApi.Dto.Request;
using BankApi.Enums;
using BankApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers
{
    [Route("api/admin/permissions")]
    [ApiController]
    [Authorize]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HasPermission(Permissions.ViewPermissions)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PermissionDto>>> GetAllPermissions()
        {
            return Ok(await _permissionService.GetAllPermissionsAsync());
        }

        [HasPermission(Permissions.ViewPermissions)]
        [HttpGet("{roleId}/assigned-permissions")]
        public async Task<ActionResult<IEnumerable<PermissionDto>>> GetPermissionsByRole(int roleId)
        {
            return Ok(await _permissionService.GetPermissionsByRoleAsync(roleId));
        }

        [HasPermission(Permissions.AssignPermissions)]
        [HttpPost("assign")]
        public async Task<IActionResult> AssignPermissions([FromBody] AssignPermissionDto request)
        {
            await _permissionService.AssignPermissionsAsync(request.RoleId, request.PermissionId);
            return Ok("Permissions assigned successfully.");
        }

        [HasPermission(Permissions.AssignPermissions)]
        [HttpPost("remove")]
        public async Task<IActionResult> RemovePermissions([FromBody] AssignPermissionDto request)
        {
            await _permissionService.RemovePermissionsAsync(request.RoleId, request.PermissionId);
            return Ok("Permissions removed successfully.");
        }
    }
}
