namespace BankApi.Dto.Request
{
    public class ApproveAccountDto
    {
        public int UserId { get; set; }
        public bool IsApproved { get; set; }
    }
}
