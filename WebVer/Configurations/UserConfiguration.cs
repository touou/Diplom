using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebVer.Domain.Identity;

namespace WebVer.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        var hasher = new PasswordHasher<User>();

        var admin = new User
        {
            Id = Guid.Parse("a0347f0f-831a-4852-a2c2-ff19d4906e8d"),
            UserName = "admin@mail.ru",
            Name = "Danek",
            NormalizedUserName = "ADMIN@MAIL.RU",
            Email = "admin@mail.RU",
            NormalizedEmail = "ADMIN@MAIL.RU",
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString(),
        };
        
        admin.PasswordHash = hasher.HashPassword(admin, "admin123");

        builder.HasData(admin);
    }
}