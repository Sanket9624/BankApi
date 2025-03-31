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

    public enum TransactionStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class Transactions
    {
        [Key]
        public int TransactionId { get; set; }
        public int? SenderAccountId { get; set; }
        public int? ReceiverAccountId { get; set; }

        [Column(TypeName = "nvarchar(10)")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TransactionType Type { get; set; }

        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "nvarchar(10)")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TransactionStatus Status { get; set; } = TransactionStatus.Pending;

        public string? Reason { get; set; }

        [ForeignKey("SenderAccountId")]
        public virtual Account? SenderAccount { get; set; }  // Nullable to avoid mapping issues

        [ForeignKey("ReceiverAccountId")]
        public virtual Account? ReceiverAccount { get; set; }  // Nullable to avoid mapping issues
    }
}
