using BankApi.Data;
using BankApi.Dto.Request;
using BankApi.Entities;
using BankApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace BankApi.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly BankDb1Context _context;

        public PermissionService(BankDb1Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync()
        {
            return await _context.Permissions
                .Select(p => new PermissionDto { PermissionId = p.PermissionId, PermissionName = p.PermissionName })
                .ToListAsync();
        }

        public async Task<IEnumerable<PermissionDto>> GetPermissionsByRoleAsync(int roleId)
        {
            return await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => new PermissionDto
                {
                    PermissionId = rp.Permission.PermissionId,
                    PermissionName = rp.Permission.PermissionName
                })
                .ToListAsync();
        }

        public async Task<bool> AssignPermissionsAsync(int roleId, List<int> permissionIds)
        {
            foreach (var pid in permissionIds)
            {
                if (!await _context.RolePermissions.AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == pid))
                {
                    _context.RolePermissions.Add(new RolePermission { RoleId = roleId, PermissionId = pid });
                }
            }

            await _context.SaveChangesAsync();
            return true;    
        }

        public async Task<bool> RemovePermissionsAsync(int roleId, List<int> permissionIds)
        {
            var toRemove = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId && permissionIds.Contains(rp.PermissionId))
                .ToListAsync();

            _context.RolePermissions.RemoveRange(toRemove);
            await _context.SaveChangesAsync();

            return true;
        }
    }

}
