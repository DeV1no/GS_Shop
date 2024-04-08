using Microsoft.EntityFrameworkCore;
using GS_Shop_UserManagement.Domain.Common;
using GS_Shop_UserManagement.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;
using GS_Shop_UserManagement.Domain.Configurations;

namespace GS_Shop_UserManagement.Persistence
{
    public class GSShopUserManagementDbContext : DbContext
    {
        public GSShopUserManagementDbContext(DbContextOptions<GSShopUserManagementDbContext> options)
            : base(options)
        {
        }

     

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserClaim> UserClaims { get; set; }
        public DbSet<UserClaimLimitation> UserLimitationClaims { get; set; }
        public DbSet<FileDetails> FileDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GSShopUserManagementDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
            new UserConfiguration(modelBuilder.Entity<User>());
            new RoleConfiguration(modelBuilder.Entity<Role>());
            new UserClaimConfiguration(modelBuilder.Entity<UserClaim>());
            new UserClaimLimitationConfiguration(modelBuilder.Entity<UserClaimLimitation>());

        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                entry.Entity.LastModifiedDate = DateTime.Now;
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.DateCreated = DateTime.Now;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}