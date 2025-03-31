using BankApi.Entities;

public class TransactionDto
{
    public int TransactionId { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public string Description { get; set; }
    public DateTime TransactionDate { get; set; }
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    public string? Reason { get; set; }
    public string? ReceiverName { get; set; } // New Property
    public string? SenderName { get; set; } // New Property
}
