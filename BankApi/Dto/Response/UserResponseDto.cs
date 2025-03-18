using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using BankApi.Entities;

namespace BankApi.Dto
{
    public class UserResponseDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string Address { get; set; }
        public string RoleName { get; set; }
        public DateTime DateOfBirth { get; set; }
        [Column(TypeName = "nvarchar(10)")] // Store Enum as String
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AccountType AccountType { get; set; }
        public RequestStatus RequestStatus { get; set; }
    }

}
