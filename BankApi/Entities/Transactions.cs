using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BankApi.Entities
{
    public enum TransactionType
    {
        Deposit,
        Withdraw,
        Transfer
    }

    public class Transactions
    {
        [Key]
        public int TransactionId { get; set; }
        public int? SenderAccountId { get; set; }  // Nullable for deposits
        public int? ReceiverAccountId { get; set; } // Nullable for withdrawals

        [Column(TypeName = "nvarchar(10)")] // Store Enum as String
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        [ForeignKey("SenderAccountId")]
        public virtual Account SenderAccount { get; set; }

        [ForeignKey("ReceiverAccountId")]
        public virtual Account ReceiverAccount { get; set; }
    }
}
