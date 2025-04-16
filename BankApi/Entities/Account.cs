using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BankApi.Entities
{
    public enum AccountType
    {
        Savings,
        Current
    }

    public class Account
    {
        [Key]
        public int AccountId { get; set; }

        [Required, MaxLength(16)]
        public string AccountNumber { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public Users Users { get; set; }

        [Required]
        public decimal Balance { get; set; }

        [Column(TypeName = "nvarchar(10)")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [Required]
        public AccountType AccountType { get; set; }

        // 🔥 New: BranchId + Navigation
        [Required]
        public int BranchId { get; set; }

        [ForeignKey("BranchId")]
        public Branch Branch { get; set; }
    }

}
