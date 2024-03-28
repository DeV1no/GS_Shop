using Microsoft.EntityFrameworkCore;
using GS_Shop_UserManagement.Domain.Common;
using GS_Shop_UserManagement.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GSShopUserManagementDbContext).Assembly);

            // Explicitly ignore the Claims property of User entity
            // modelBuilder.Entity<User>().Ignore(u => u.Claims);
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