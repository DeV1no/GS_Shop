using GS_Shop_UserManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GS_Shop_UserManagement.Domain.Configurations;

public class UserClaimLimitationConfiguration
{
    public UserClaimLimitationConfiguration(EntityTypeBuilder<UserClaimLimitation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ClaimLimitationValue).IsRequired();
        builder.HasOne(x => x.User).WithMany(x => x.UserClaimLimitations);
    }
}