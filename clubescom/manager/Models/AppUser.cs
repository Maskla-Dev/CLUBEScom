using Microsoft.AspNetCore.Identity;

namespace clubescom.manager.models;

public class AppUser : IdentityUser
{
    public Guid ID { get; set; }
    public string Name { get; set; }
    public string ProfileImagePath { get; set; }
    public ICollection<IdentityUserRole<Guid>> Roles { get; set; }
}