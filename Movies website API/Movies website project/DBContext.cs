using Microsoft.EntityFrameworkCore;
using Movies_website_project.Entities;

public class DBContext : DbContext
{
    public DBContext(DbContextOptions<DBContext> options)
    : base(options)
    {

    }
    public DbSet<User> Users { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Actor>actors { get; set; }
    public DbSet<Review> reviews { get; set; }
}