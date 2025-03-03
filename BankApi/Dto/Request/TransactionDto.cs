using BankApi.Entities;

public class TransactionDto
{
    public int TransactionId { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public DateTime TransactionDate { get; set; }
    public string? ReceiverName { get; set; } // New Property
}
