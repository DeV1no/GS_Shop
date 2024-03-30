using GS_Shop_UserManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GS_Shop_UserManagement.Domain.Configurations;

public class UserConfiguration
{
    public UserConfiguration(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(x => x.Id);
        builder.Property(x => x.Email).IsRequired();
        builder.Property(x => x.FirstName).IsRequired();
        builder.Property(x => x.LastName).IsRequired();
        builder.HasMany(x => x.Roles).WithMany(x => x.Users);
        builder.HasMany(x => x.UserClaims).WithOne(x => x.User).HasForeignKey(x => x.UserId);
        builder.HasMany(x => x.UserClaimLimitations).WithOne(q => q.User).HasForeignKey(x => x.UserId);
    }
}