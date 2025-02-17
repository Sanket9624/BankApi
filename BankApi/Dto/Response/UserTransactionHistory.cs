namespace BankApi.Dto
{
    public class UserTransactionHistory
    {
        public int TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }  // "Deposit" or "Withdraw"
        public DateTime TransactionDate { get; set; }
    }
}
