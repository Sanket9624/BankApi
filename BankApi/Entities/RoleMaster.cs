using System.ComponentModel.DataAnnotations;

namespace BankApi.Entities
{
    public class RoleMaster
    {
        [Key]
        public int RoleId { get; set; }

        [Required, MaxLength(50)]
        public string RoleName { get; set; }

        public ICollection<Users> Users { get; set; }
    }

}
