using System.Collections.Generic;
using clubescom.manager.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace clubescom.manager;

public class DBInitializer
{
    
    public static async Task initBase(AppDbContext context, UserManager<AppUser> _managerAgent)
    {
        var roles = new List<Role>
        {
            new Role { Name = "Admin" },
            new Role { Name = "User" }
        };
        foreach (var role in roles)
        {
            context.Roles.Add(role);
        }
        var postTypes = new List<PostType>
        {
            new PostType { Name = "News" },
            new PostType { Name = "Event" },
            new PostType { Name = "Article" }
        };
        foreach (var postType in postTypes)
        {
            context.PostTypes.Add(postType);
        }
        await context.SaveChangesAsync();
        // get "admin" role form db
        var adminRole = await context.Roles.Select(r => r).Where(r => r.Name == "Admin").FirstOrDefaultAsync();
        var userRole = await context.Roles.Select(r => r).Where(r => r.Name == "User").FirstOrDefaultAsync();
        var users = new List<AppUser>
        {
            new AppUser
            {
                UserName = "admin", Email = "root@root.com", Name = "Hanba Morelos",
                Roles = adminRole, PhoneNumber = "5555555555", ProfileImagePath = "default-avatar.jpg"
            },
            new AppUser
            {
                UserName = "user1", Email = "user1@user.com", Name = "Luis Perera",
                Roles = userRole, PhoneNumber = "5555555555", ProfileImagePath = "default-avatar.jpg"
            },
            new AppUser
            {
                UserName = "user2", Email = "user2@user.com", Name = "Alicia Santos",
                Roles = userRole, PhoneNumber = "5555555555", ProfileImagePath = "default-avatar.jpg"
            }
        };
        foreach (var user in users)
        {
            var result = await _managerAgent.CreateAsync(user, "123456");
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"Cannot create user: {error.Description}");
                }
            }
        }
        await context.SaveChangesAsync();
    }
    
    
}