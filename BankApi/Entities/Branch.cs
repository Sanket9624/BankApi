using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankApi.Entities
{
    public class Branch
    {
        [Key]
        public int? BranchId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Address { get; set; }

        [Required]
        public int BankId { get; set; }

        [ForeignKey("BankId")]
        public Bank Bank { get; set; }

        public ICollection<Users> Users { get; set; }
        public ICollection<Account> Accounts { get; set; }
    }

}
