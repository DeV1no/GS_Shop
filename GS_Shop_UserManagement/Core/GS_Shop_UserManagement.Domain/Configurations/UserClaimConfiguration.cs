using GS_Shop_UserManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GS_Shop_UserManagement.Domain.Configurations;

public class UserClaimConfiguration
{
    public UserClaimConfiguration(EntityTypeBuilder<UserClaim> builder)
    {
        builder.HasIndex(x => x.Id);
        builder.Property(x => x.ClaimType).IsRequired();
        builder.HasOne(x => x.User).WithMany(x => x.UserClaims);

    }
}