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
        public string SenderAccount { get; set; }
        public string ReceiverAccount { get; set; }
        public decimal Amount { get; set; }
        public TransactionStatus status { get; set; }
        public string Description { get; set; }
        public string Reason { get; set; }
        public TransactionType Type { get; set; }  // Deposit, Withdraw, Transfer
        public DateTime TransactionDate { get; set; }
    }
}
