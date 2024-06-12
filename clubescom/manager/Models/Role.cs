using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace clubescom.manager.models;

public class Role : IdentityRole<Guid>
{
    public ICollection<IdentityUserRole<Guid>> UserRoles { get; set; }
}