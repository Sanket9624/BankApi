using BankApi.Dto.Request;

namespace BankApi.Services.Interfaces
{
    public interface IPermissionService
    {
        Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync();
        Task<IEnumerable<PermissionDto>> GetPermissionsByRoleAsync(int roleId);
        Task<bool> AssignPermissionsAsync(int roleId, List<int> permissionIds);
        Task<bool> RemovePermissionsAsync(int roleId, List<int> permissionIds);
    }

}
