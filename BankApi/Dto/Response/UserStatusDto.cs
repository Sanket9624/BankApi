using BankApi.Entities;

namespace BankApi.Dto.Response
{
    public class UserStatusDto
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public RequestStatus RequestStatus { get; set; }
        public bool IsEmailVerified { get; set; }
        public string? RejectionReason { get; set; }

        public string RoleName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
