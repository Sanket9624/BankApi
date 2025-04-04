﻿using System.ComponentModel.DataAnnotations;

namespace BankApi.Entities
{
    public class RoleMaster
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        [StringLength(50)]
        public string RoleName { get; set; } = string.Empty;

        public ICollection<RolePermissions> RolePermissions { get; set; } = new List<RolePermissions>();

        public ICollection<Users> Users { get; set; }
    }

}
