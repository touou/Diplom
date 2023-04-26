using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebVer.Domain.Identity;

namespace WebVer.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasData(
            new UserRole
            {
                RoleId = Guid.Parse("3a63d931-d70a-4439-8e7f-486943f8bf5d"),
                UserId = Guid.Parse("a0347f0f-831a-4852-a2c2-ff19d4906e8d")
            }
        );
    }
}