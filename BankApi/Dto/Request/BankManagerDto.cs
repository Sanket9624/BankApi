﻿using System.Reflection.Metadata;
using BankApi.Entities;

namespace BankApi.Dto
{
    public class BankManagerDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string MobileNo { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int RoleId { get; set; }
        public RequestStatus RequestStatus { get; set; } = RequestStatus.Approved;
    }
}
