using Microsoft.EntityFrameworkCore;

public class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> opt) : base(opt) { }

    public DbSet<UserModel> Users { get; set; }
}