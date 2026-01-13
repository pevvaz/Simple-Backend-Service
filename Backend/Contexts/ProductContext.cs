using Microsoft.EntityFrameworkCore;

public class ProductsContext : DbContext
{
    public ProductsContext(DbContextOptions<ProductsContext> options) : base(options) { }

    public DbSet<ProductsModels> Products { get; set; }
}