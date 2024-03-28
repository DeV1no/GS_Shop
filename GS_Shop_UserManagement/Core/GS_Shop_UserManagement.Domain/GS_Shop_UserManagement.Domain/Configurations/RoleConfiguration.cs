using GS_Shop_UserManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GS_Shop_UserManagement.Domain.Configurations;

public class RoleConfiguration
{
    public RoleConfiguration(EntityTypeBuilder<Role> builder)
    {
        builder.HasIndex(x => x.Id);
        builder.Property(x => x.Name).IsRequired();
        builder.HasMany(x => x.Users).WithMany(x => x.Roles);
    }
}