using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using BankApi.Entities;

namespace BankApi.Dto
{
    public class UserRequestDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string MobileNo { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public AccountType AccountType { get; set; }
    }

}
