namespace BankApi.Dto
{
    public class TransactionDetails
    {
        public int TransactionId { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string AccountNumber { get; set;}
        public string AccountType { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } 
        public DateTime TransactionDate { get; set; }
    }
}
