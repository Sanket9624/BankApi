namespace BankApi.Dto
{
    public class AccountSummaryDto
    {
        public int TotalAccounts { get; set; }
        public List<AccountDto> AccountDetails { get; set; } = new();
    }
}
