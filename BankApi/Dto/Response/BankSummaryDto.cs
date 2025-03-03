namespace BankApi.Dto
{
    public class BankSummaryDto
    {
        public decimal TotalBankBalance { get; set; }
        public decimal TotalDepositedMoney { get; set; }
        public decimal TotalWithdrawnMoney { get; set; }
        public int TotalTransactions { get; set; }
        public List<UserTransactionCountDto> UserTransactionCounts { get; set; } = new();
    }

    public class UserTransactionCountDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int TransactionCount { get; set; }
    }
}
