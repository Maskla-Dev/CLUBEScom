using Microsoft.AspNetCore.Identity;

namespace clubescom.manager.models;

public class AppUser : IdentityUser
{
    public string Name { get; set; }
    public string ProfileImagePath { get; set; }
    public Role Roles { get; set; }
}