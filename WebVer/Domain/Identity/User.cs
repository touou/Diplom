using Microsoft.AspNetCore.Identity;

namespace WebVer.Domain.Identity;

public class User : IdentityUser<Guid>
{
    public string Name { get; set; }
    public virtual ICollection<UserRole> UserRoles { get; set; }
}