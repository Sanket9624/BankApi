using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using BankApi.Entities;

namespace BankApi.Dto
{
    public class AccountDto
    {
        //[JsonIgnore]
        //public int AccountId { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public AccountType AccountType { get; set; }
    }
}
