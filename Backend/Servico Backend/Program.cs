using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddDbContext<UsersContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("users"));
});

var app = builder.Build();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<UsersContext>().Database.Migrate();
}
app.Run();
