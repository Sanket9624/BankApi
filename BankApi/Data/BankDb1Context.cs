using Microsoft.EntityFrameworkCore;
using BankApi.Entities;

namespace BankApi.Data
{
    public class BankDb1Context : DbContext
    {
        public BankDb1Context(DbContextOptions<BankDb1Context> options) : base(options)
        {

        }
        public DbSet<Users> Users{ get; set; }
        public DbSet<RoleMaster> RoleMaster { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<Transactions> Transactions { get; set; }

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

            modelBuilder.Entity<Users>()
        .HasOne(u => u.RoleMaster)
        .WithMany(r => r.Users)
        .HasForeignKey(u => u.RoleId)
        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RoleMaster>().HasData(
                new RoleMaster { RoleId = 1, RoleName = "SuperAdmin" },
                new RoleMaster { RoleId = 2, RoleName = "BankManager" },
                new RoleMaster { RoleId = 3, RoleName = "Customer" }
            );
        }
    }
}