using BankApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Policy = "SuperAdminOnly")]
[Route("api/admin/permissions")]
[ApiController]
public class PermissionController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    public PermissionController(IPermissionService permissionService)
    {
        _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PermissionResponseDto>>> GetPermissions()
    {
        var permissions = await _permissionService.GetPermissionsAsync(HttpContext); // ✅ Pass HttpContext
        return Ok(permissions);
    }


    [HttpPost]
    public async Task<ActionResult<PermissionResponseDto>> CreatePermission([FromBody] CreatePermissionDto permissionDto)
    {
        var createdPermission = await _permissionService.CreatePermissionAsync(permissionDto);
        return CreatedAtAction(nameof(GetPermissions), createdPermission);
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignPermissionsToRole([FromBody] AssignPermissionsDto assignDto)
    {
        if (assignDto == null || !assignDto.PermissionId.Any())
        {
            return BadRequest("Invalid data. At least one permission must be assigned.");
        }

        var result = await _permissionService.AssignPermissionsToRoleAsync(assignDto.RoleId, assignDto.PermissionId);

        if (!result)
        {
            return BadRequest("Failed to assign permissions.");
        }

        return Ok("Permissions assigned successfully.");
    }

    [HttpGet("{roleId}/assigned-permissions")]
    public async Task<ActionResult<IEnumerable<string>>> GetPermissionsByRoleId(int roleId)
    {
        var permissions = await _permissionService.GetPermissionsByRoleIdAsync(roleId);

        if (permissions == null)
        {
            return NotFound($"Role ID {roleId} not found");
        }
        else if (!permissions.Any())
        {
            return Ok(new List<string>()); // Return an empty list if no permissions found
        }

        return Ok(permissions);
    }
    [HttpPost("remove")]
    public async Task<IActionResult> RemovePermissionsFromRole([FromBody] AssignPermissionsDto removeDto)
    {
        if (removeDto == null || !removeDto.PermissionId.Any())
        {
            return BadRequest("Invalid data. At least one permission must be removed.");
        }

        var result = await _permissionService.RemovePermissionsFromRoleAsync(removeDto.RoleId, removeDto.PermissionId);

        if (!result)
        {
            return BadRequest("Failed to remove permissions.");
        }

        return Ok("Permissions removed successfully.");
    }


}
