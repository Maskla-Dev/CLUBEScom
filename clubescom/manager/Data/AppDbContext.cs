using Microsoft.EntityFrameworkCore;
using clubescom.manager.models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace clubescom.manager;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public DbSet<Calendar> Calendars { get; set; }
    public DbSet<AppUser> Users { get; set; }
    public DbSet<PostType> PostTypes { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Club> Clubs { get; set; }
    public DbSet<Role> Roles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=ClubesComDb.db");
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}