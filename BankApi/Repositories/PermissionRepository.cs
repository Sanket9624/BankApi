using BankApi.Data;
using BankApi.Entities;
using BankApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace BankApi.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly BankDb1Context _context;

        public PermissionRepository(BankDb1Context context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Permissions>> GetPermissionsAsync()
        {
            return await _context.Permissions.ToListAsync();
        }

        public async Task<List<string>> GetPermissionsByUserId(int userId)
        {
            return await _context.RolePermissions
                .Where(rp => _context.Users
                    .Where(u => u.UserId == userId)
                    .Select(u => u.RoleId)
                    .Contains(rp.RoleId))
                .Select(rp => rp.Permissions.PermissionName)
                .Distinct()
                .ToListAsync();
        }
        public async Task<List<Permissions>> GetPermissionsByRoleId(int roleId)
        {
            return await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => new Permissions
                {
                    PermissionId = rp.Permissions.PermissionId,  // ✅ Include PermissionId
                    PermissionName = rp.Permissions.PermissionName
                })
                .ToListAsync();
        }


        public async Task<Permissions> CreatePermissionAsync(string permissionName)
        {
            var permission = new Permissions
            {
                PermissionName = permissionName
            };
            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();
            return permission;
        }
        public async Task<List<Permissions>> GetPermissionsByIdsAsync(List<int> permissionIds)
        {
            return await _context.Permissions
                .Where(p => permissionIds.Contains(p.PermissionId))
                .ToListAsync();
        }

        public async Task AssignPermissionsToRoleAsync(int roleId, List<int> permissionIds)
        {
            foreach (var permissionId in permissionIds)
            {
                var rolePermission = new RolePermissions
                {
                    RoleId = roleId,
                    PermissionId = permissionId
                };
                _context.RolePermissions.Add(rolePermission);
            }

            await _context.SaveChangesAsync();
        }
        public async Task RemovePermissionsFromRoleAsync(int roleId, List<int> permissionIds)
        {
            var rolePermissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId && permissionIds.Contains(rp.PermissionId))
                .ToListAsync();

            if (rolePermissions.Any())
            {
                _context.RolePermissions.RemoveRange(rolePermissions);
                await _context.SaveChangesAsync();
            }
        }

    }
}
