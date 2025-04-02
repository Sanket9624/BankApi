using Microsoft.EntityFrameworkCore;
using BankApi.Entities;
using System.Security;
using BankApi.Enums;

namespace BankApi.Data
{
    public class BankDb1Context : DbContext
    {
        public BankDb1Context(DbContextOptions<BankDb1Context> options) : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<RoleMaster> RoleMaster { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<Transactions> Transactions { get; set; }
        public DbSet<OtpVerifications> OtpVerifications { get; set; }
        public DbSet<Permissions> Permissions { get; set; }
        public DbSet<RolePermissions> RolePermissions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
                optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>()
                .HasOne(u => u.RoleMaster)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Users>()
                .HasOne(u => u.Account)
                .WithOne(a => a.Users)
                .HasForeignKey<Account>(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Transactions>()
                .HasOne(t => t.SenderAccount)
                .WithMany()
                .HasForeignKey(t => t.SenderAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transactions>()
                .HasOne(t => t.ReceiverAccount)
                .WithMany()
                .HasForeignKey(t => t.ReceiverAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seeding initial data
            modelBuilder.Entity<RoleMaster>().HasData(
                new RoleMaster { RoleId = 1, RoleName = "SuperAdmin" },
                new RoleMaster { RoleId = 2, RoleName = "BankManager" },
                new RoleMaster { RoleId = 3, RoleName = "Customer" }
            );

            modelBuilder.Entity<Permissions>().HasData(
                new Permissions { PermissionId = (int)PermissionEnum.CreateRole, PermissionName = PermissionEnum.CreateRole.ToString() },
                new Permissions { PermissionId = (int)PermissionEnum.DeleteRole, PermissionName = PermissionEnum.DeleteRole.ToString() },
                new Permissions { PermissionId = (int)PermissionEnum.ViewRoles, PermissionName = PermissionEnum.ViewRoles.ToString() },

                new Permissions { PermissionId = (int)PermissionEnum.CreateManager, PermissionName = PermissionEnum.CreateManager.ToString() },
                new Permissions { PermissionId = (int)PermissionEnum.DeleteManager, PermissionName = PermissionEnum.DeleteManager.ToString() },
                new Permissions { PermissionId = (int)PermissionEnum.VerifyManager, PermissionName = PermissionEnum.VerifyManager.ToString() },

                new Permissions { PermissionId = (int)PermissionEnum.ApproveAccount, PermissionName = PermissionEnum.ApproveAccount.ToString() },
                new Permissions { PermissionId = (int)PermissionEnum.ViewUsers, PermissionName = PermissionEnum.ViewUsers.ToString() },

                new Permissions { PermissionId = (int)PermissionEnum.UpdateUser, PermissionName = PermissionEnum.UpdateUser.ToString() },
                new Permissions { PermissionId = (int)PermissionEnum.DeleteUser, PermissionName = PermissionEnum.DeleteUser.ToString() },

                new Permissions { PermissionId = (int)PermissionEnum.ApproveTransaction, PermissionName = PermissionEnum.ApproveTransaction.ToString() },
                new Permissions { PermissionId = (int)PermissionEnum.RejectTransaction, PermissionName = PermissionEnum.RejectTransaction.ToString() },
                new Permissions { PermissionId = (int)PermissionEnum.ViewPendingTransactions, PermissionName = PermissionEnum.ViewPendingTransactions.ToString() },

                new Permissions { PermissionId = (int)PermissionEnum.BankSummary, PermissionName = PermissionEnum.BankSummary.ToString() },
                new Permissions { PermissionId = (int)PermissionEnum.GetUserAccountDetails, PermissionName = PermissionEnum.GetUserAccountDetails.ToString() },
                new Permissions { PermissionId = (int)PermissionEnum.GetUserDetailsByAccountNumber, PermissionName = PermissionEnum.GetUserDetailsByAccountNumber.ToString() },

                new Permissions { PermissionId = (int)PermissionEnum.GetUserDetailsByEmail, PermissionName = PermissionEnum.GetUserDetailsByEmail.ToString() },
                new Permissions { PermissionId = (int)PermissionEnum.GetTotalAccounts, PermissionName = PermissionEnum.GetTotalAccounts.ToString() },
                new Permissions { PermissionId = (int)PermissionEnum.GetTransactions, PermissionName = PermissionEnum.GetTransactions.ToString() },

                new Permissions { PermissionId = (int)PermissionEnum.MakeDeposit, PermissionName = PermissionEnum.MakeDeposit.ToString() },
                new Permissions { PermissionId = (int)PermissionEnum.MakeWithdrawal, PermissionName = PermissionEnum.MakeWithdrawal.ToString() },
                new Permissions { PermissionId = (int)PermissionEnum.MakeTransfer, PermissionName = PermissionEnum.MakeTransfer.ToString() },
                new Permissions { PermissionId = (int)PermissionEnum.ViewBalance, PermissionName = PermissionEnum.ViewBalance.ToString() },
                new Permissions { PermissionId = (int)PermissionEnum.ViewTransactions, PermissionName = PermissionEnum.ViewTransactions.ToString() },
                new Permissions { PermissionId = (int)PermissionEnum.ViewCustomTransactions, PermissionName = PermissionEnum.ViewCustomTransactions.ToString() }
            );


            // Defining many-to-many relationship between RoleMaster and Permissions
            modelBuilder.Entity<RolePermissions>().HasKey(rp => new { rp.RoleId, rp.PermissionId });

            modelBuilder.Entity<RolePermissions>()
                .HasOne(rp => rp.RoleMaster)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId);

            modelBuilder.Entity<RolePermissions>()
                .HasOne(rp => rp.Permissions)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId);
        }

    }
}
