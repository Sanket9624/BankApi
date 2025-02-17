using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using BankApi.Entities;

namespace BankApi.Dto
{
    public class TransactionResponseDto
    {
        public int TransactionId { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }  // Deposit, Withdraw, Transfer
        public DateTime TransactionDate { get; set; }
    }
}
