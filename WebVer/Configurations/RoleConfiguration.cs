using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebVer.Domain.Identity;

namespace WebVer.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasData(
            new Role
            {
                Id = Guid.Parse("3a63d931-d70a-4439-8e7f-486943f8bf5d"),
                Name = "Админ",
                NormalizedName = "АДМИН",
            },
            new Role
            {
                Id = Guid.Parse("3ab1e52e-5722-417e-ba7d-ac50c19ff6fc"),
                Name = "Пользователь",
                NormalizedName = "ПОЛЬЗОВАТЕЛЬ",
            }
        );
    }
}