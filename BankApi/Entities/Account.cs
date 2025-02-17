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
        public string AccountNumber { get; set; } // Will be set from Service

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public Users Users { get; set; }

        [Required]
        public decimal Balance { get; set; }

        [Column(TypeName = "nvarchar(10)")] // Store Enum as String
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [Required]
        public AccountType AccountType { get; set; }

    }
}
