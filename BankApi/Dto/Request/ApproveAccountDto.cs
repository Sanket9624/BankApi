using BankApi.Entities;

namespace BankApi.Dto.Request
{
    public class ApproveAccountDto
    {
        public int UserId { get; set; }
        public bool IsApproved { get; set; }
        public string? RejectedReason { get; set; }    
        public AccountType AccountType { get; set; }
    }
}
