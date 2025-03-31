using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BankApi.Entities
{
    public enum RequestStatus
    {
        Pending,
        Approved,
        Rejected
    }

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
        public string PasswordHash { get; set; } // Properly hashed

        [Required, MaxLength(15)]
        public string MobileNo { get; set; }

        [Required, MaxLength(200)]
        public string Address { get; set; }

        public DateTime DateOfBirth { get; set; }

        [Column(TypeName = "nvarchar(10)")] // Store Enum as String
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AccountType AccountType { get; set; }
        [Required]
        public int RoleId { get; set; } // Linked to Role Table
        public bool IsEmailVerified { get; set; } = false;
        public bool TwoFactorEnabled { get; set; } = false;
        public DateTime? ApprovedAt { get; set; }
        public DateTime? RejectedAt { get; set; }
        public string? RejectionReason { get; set; }

        [Column(TypeName = "nvarchar(10)")] // Store Enum as String
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RequestStatus RequestStatus { get; set; } = RequestStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        [ForeignKey("RoleId")]
        public RoleMaster RoleMaster { get; set; }
        public Account Account { get; set; }

    }
}
