using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;
using BankApi.Entities;

namespace BankApi.Repositories.Interfaces
{
    public interface IPermissionRepository
    {
        Task<IEnumerable<Permissions>> GetPermissionsAsync();
        Task<List<string>> GetPermissionsByUserId(int userId);
        Task<List<Permissions>> GetPermissionsByRoleId(int roleId);
        Task<List<Permissions>> GetPermissionsByIdsAsync(List<int> permissionIds);
        Task<Permissions> CreatePermissionAsync(string permissionName);
        Task AssignPermissionsToRoleAsync(int roleId, List<int> permissionIds);
        Task RemovePermissionsFromRoleAsync(int roleId, List<int> permissionIds);
    }
}
