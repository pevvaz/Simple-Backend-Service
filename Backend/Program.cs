using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5050");

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddLogging();
builder.Services.AddDbContext<UserContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("users"));
});
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultScheme = "main_scheme";
}).AddJwtBearer("main_scheme", opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,

        ValidAudiences = ["admin", "customer"],
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!))
    };
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<UserContext>().Database.Migrate();
}
app.Run();
