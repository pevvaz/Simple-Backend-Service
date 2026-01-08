using Microsoft.EntityFrameworkCore;

public class UsersContext : DbContext
{
    public UsersContext(DbContextOptions<UsersContext> opt) : base(opt) { }

    public DbSet<UsersModels> Users { get; set; }
}