using Microsoft.AspNetCore.Identity;

namespace WebVer.Domain.Identity;

public class Role : IdentityRole<Guid>
{
    public virtual ICollection<UserRole> UserRoles { get; set; }
}