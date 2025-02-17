namespace BankApi.Dto
{
    public class UserWithAccountDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public AccountDto Account { get; set; }
    }
}
