using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5050");

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddLogging();
builder.Services.AddDbContext<UsersContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("users"));
});
builder.Services.AddDbContext<ProductsContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("products"));
});

var app = builder.Build();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<UsersContext>().Database.Migrate();
}
app.Run();
