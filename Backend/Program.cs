using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5050");

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddLogging();
builder.Services.AddDbContext<UserContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("users"));
});

var app = builder.Build();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<UserContext>().Database.Migrate();
}
app.Run();
