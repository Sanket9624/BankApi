using System.ComponentModel.DataAnnotations;

namespace BankApi.Entities
{
    public class Permissions
    {
        [Key]
        public int PermissionId { get; set; }

        [Required]
        [StringLength(100)]
        public string PermissionName { get; set; } = string.Empty;
        public ICollection<RolePermissions> RolePermissions { get; set; } = new List<RolePermissions>();
    }
}
