namespace BankApi.Dto
{
    public class UserTransactionSummary
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public decimal TotalDeposited { get; set; }
        public decimal TotalWithdrawn { get; set; }
    }
}
