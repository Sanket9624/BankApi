using System.Data;

namespace BankApi.Entities
{
    public class RolePermission
    {
        public int RoleId { get; set; }
        public RoleMaster Role { get; set; }

        public int PermissionId { get; set; }
        public Permission Permission { get; set; }
    }

}