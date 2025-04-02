using BankApi.Dto.Response;
using BankApi.Dto.Request;
using BankApi.Repositories.Interfaces;
using BankApi.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankApi.Entities;
using System.Security.Claims;

namespace BankApi.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAdminRepository _adminRepository;

        public PermissionService(IPermissionRepository permissionRepository, IUserRepository userRepository, IAdminRepository adminRepository)
        {
            _permissionRepository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
            _userRepository = userRepository;
            _adminRepository = adminRepository;
        }

        public async Task<IEnumerable<PermissionResponseDto>> GetPermissionsAsync(HttpContext httpContext)
        {
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) throw new Exception("User ID not found in token");

            int userId = int.Parse(userIdClaim.Value);

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            var permissions = await _permissionRepository.GetPermissionsByRoleId(user.RoleId);
            return permissions.Select(p => new PermissionResponseDto
            {
                PermissionId = p.PermissionId,
                PermissionName = p.PermissionName
            }).ToList();
        }


        public async Task<PermissionResponseDto> CreatePermissionAsync(CreatePermissionDto permissionDto)
        {
            var createdPermission = await _permissionRepository.CreatePermissionAsync(permissionDto.PermissionName);
            return new PermissionResponseDto
            {
                PermissionId = createdPermission.PermissionId,
                PermissionName = createdPermission.PermissionName
            };
        }

        public async Task<bool> AssignPermissionsToRoleAsync(int roleId, List<int> permissionIds)
        {
            var role = await _adminRepository.GetRoleByIdAsync(roleId);
            if (role == null) throw new Exception("Role not found");

            var permissions = await _permissionRepository.GetPermissionsByIdsAsync(permissionIds);
            if (permissions.Count != permissionIds.Count)
                throw new Exception("One or more permissions do not exist");

            await _permissionRepository.AssignPermissionsToRoleAsync(roleId, permissionIds);
            return true;
        }
        public async Task<bool> RemovePermissionsFromRoleAsync(int roleId, List<int> permissionIds)
        {
            var role = await _adminRepository.GetRoleByIdAsync(roleId);
            if (role == null) throw new Exception("Role not found");

            var permissions = await _permissionRepository.GetPermissionsByIdsAsync(permissionIds);
            if (permissions.Count != permissionIds.Count)
                throw new Exception("One or more permissions do not exist");

            await _permissionRepository.RemovePermissionsFromRoleAsync(roleId, permissionIds);
            return true;
        }

        public async Task<List<Permissions>> GetPermissionsByRoleIdAsync(int roleId)
        {
            var permissions = await _permissionRepository.GetPermissionsByRoleId(roleId);
            return permissions.Select(p => new Permissions
            {
                PermissionId = p.PermissionId,   // ✅ Include PermissionId
                PermissionName = p.PermissionName
            }).ToList();
        }

    }
}
