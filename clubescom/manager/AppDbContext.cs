using Microsoft.EntityFrameworkCore;
using clubescom.manager.models;

namespace clubescom.manager;

public class AppDbContext : DbContext
{
    public DbSet<Calendar> Calendars { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Rol> Roles { get; set; }
    public DbSet<PostType> PostTypes { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Club> Clubs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=ClubesComDb.db");
    }
}