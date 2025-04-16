using Microsoft.EntityFrameworkCore;
using BankApi.Entities;

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
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<OtpVerifications> OtpVerifications { get; set; }

        // 🔥 New Entities
        public DbSet<Bank> Banks { get; set; }
        public DbSet<Branch> Branches { get; set; }

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
            // Role to User
            modelBuilder.Entity<Users>()
                .HasOne(u => u.RoleMaster)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // User to Account
            modelBuilder.Entity<Users>()
                .HasOne(u => u.Account)
                .WithOne(a => a.Users)
                .HasForeignKey<Account>(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Transactions
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

            // Role Permissions (Many-to-Many)
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId);

            // 🔥 Users ↔ Branch
            modelBuilder.Entity<Users>()
                .HasOne(u => u.Branch)
                .WithMany(b => b.Users)
                .HasForeignKey(u => u.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔥 Account ↔ Branch
            modelBuilder.Entity<Account>()
                .HasOne(a => a.Branch)
                .WithMany(b => b.Accounts)
                .HasForeignKey(a => a.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔥 Branch ↔ Bank
            modelBuilder.Entity<Branch>()
                .HasOne(b => b.Bank)
                .WithMany(bk => bk.Branches)
                .HasForeignKey(b => b.BankId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed roles
            modelBuilder.Entity<RoleMaster>().HasData(
                new RoleMaster { RoleId = 1, RoleName = "SuperAdmin" },
                new RoleMaster { RoleId = 2, RoleName = "BankManager" },
                new RoleMaster { RoleId = 3, RoleName = "Customer" }
            );
        }
    }
}
