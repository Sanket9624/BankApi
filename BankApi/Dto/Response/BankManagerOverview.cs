namespace BankApi.Dto
{
    public class BankManagerOverview
    {
        public int TotalTransactions { get; set; }
        public decimal TotalDeposited { get; set; }
        public decimal TotalWithdrawn { get; set; }
        public decimal TotalBankBalance { get; set; }
    }
}
