using BankApi.Entities;
using BankApi.Repositories.Interfaces;
using BankApi.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BankApi.Services
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly string _key = "secret-key-123-456-789-123-213ftyufuyigyfgutiygyuhyifutxcfgvjhb";

        public JwtTokenGenerator(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
        }

        public async Task<string> GenerateToken(int userId, string email, int roleId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_key);

            // ✅ Fetch permissions for the user
            var userPermissions = await _permissionRepository.GetPermissionsByUserId(userId);

            // ✅ Create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim("RoleId", roleId.ToString()) // ✅ Store role in the token
            };

            // ✅ Add permissions as claims
            foreach (var permission in userPermissions)
            {
                claims.Add(new Claim("Permission", permission));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
