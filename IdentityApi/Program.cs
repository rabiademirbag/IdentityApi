using IdentityApi.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<IdentityContext>
    (options => options.UseSqlServer
    (builder.Configuration.GetConnectionString("Default")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<IdentityContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    //password say� i�ermeli
    options.Password.RequireDigit = true;
    //en az 8 karakter olmal�
    options.Password.RequiredLength = 8;
    
    //5 dk kitlensin
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    //5 kere hatal� giri� yap�l�rsa
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.MapControllers();

app.Run();
