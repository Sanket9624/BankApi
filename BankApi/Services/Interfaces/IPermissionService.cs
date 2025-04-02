using BankApi.Dto.Response;
using BankApi.Dto.Request;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankApi.Entities;

namespace BankApi.Services.Interfaces
{
    public interface IPermissionService
    {
        Task<IEnumerable<PermissionResponseDto>> GetPermissionsAsync(HttpContext httpContext);
        Task<PermissionResponseDto> CreatePermissionAsync(CreatePermissionDto permissionDto);
        Task<bool> AssignPermissionsToRoleAsync(int roleId, List<int> permissionIds);
        Task<List<Permissions>> GetPermissionsByRoleIdAsync(int roleId);
        Task<bool> RemovePermissionsFromRoleAsync(int roleId, List<int> permissionIds);
    }
}
