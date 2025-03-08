using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Entities
{

    public class Users
    {
        [Key]
        public int UserId { get; set; }

        [Required, MaxLength(50)]
        public string FirstName { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; } // Now properly hashed

        [Required, MaxLength(15)]
        public string MobileNo { get; set; }

        [Required, MaxLength(200)]
        public string Address { get; set; }

        public DateTime DateOfBirth { get; set; }

        [Required]
        public int RoleId { get; set; } // Linked to Role Table

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [ForeignKey("RoleId")]
        public RoleMaster RoleMaster { get; set; }
        public Account Account { get; set; }
        public bool IsEmailVerified { get; set; } = false; // For Registration OTP
        public bool TwoFactorEnabled { get; set; } = false; // For 2FA
        public string? Otp { get; set; } // Store OTP temporarily
        public DateTime? OtpExpiry { get; set; }// One-to-One with Account

    }
}
