using BankApi.Data;
using BankApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BankApi.Services
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly BankDb1Context _context;
        private readonly IConfiguration _configuration;

        public JwtTokenGenerator(BankDb1Context context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public string GenerateToken(int userId, string email, int roleId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var keyString = _configuration["JWT:Key"];
            if (string.IsNullOrEmpty(keyString))
                throw new Exception("JWT secret key is not configured.");

            var key = Encoding.ASCII.GetBytes(keyString);

            // 🔍 Fetch permissions for the user's role
            var permissions = _context.RolePermissions
                .Include(rp => rp.Permission)
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.Permission.PermissionName)
                .ToList();

            // 🛡️ Build JWT claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim("RoleId", roleId.ToString())
            };

            // ➕ Add permissions as individual claims
            claims.AddRange(permissions.Select(p => new Claim("Permission", p)));

            // 🧾 Token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
