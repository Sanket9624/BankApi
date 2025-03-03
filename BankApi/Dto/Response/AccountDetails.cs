namespace BankApi.Dto
{
    public class AccountDetails
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
