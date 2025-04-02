using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BankApi.Entities
{
    public class RolePermissions
    {
        [Key]
        public int RolePermissionId { get; set; }

        [ForeignKey("RoleMaster")]
        public int RoleId { get; set; }
        public RoleMaster RoleMaster { get; set; }

        [ForeignKey("Permissions")]
        public int PermissionId { get; set; }
        public Permissions Permissions { get; set; }
    }

}
