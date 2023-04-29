using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using AuthenticationService.Datas;
using AuthenticationService;
using System;
using Microsoft.AspNetCore.Identity;
using static OpenIddict.Abstractions.OpenIddictConstants;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors();

// Add DB
string migrationsAssembly = typeof(Program).Assembly.GetName().Name ?? string.Empty;
string connectionString
    = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
    ?? builder.Configuration.GetConnectionString("default")
    ?? throw new InvalidOperationException("Connection string not found!");

builder.Services.AddDbContext<TmpDataContext>(opt => opt
    .UseSqlServer(connectionString)
    .UseOpenIddict()
);

// Add OpendIddict
builder.Services.AddMyOpendIddictConfiguration();

// Add Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddSignInManager()
    .AddEntityFrameworkStores<TmpDataContext>()
    .AddUserManager<UserManager<IdentityUser>>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.ClaimsIdentity.UserNameClaimType = Claims.Name;
    options.ClaimsIdentity.UserIdClaimType = Claims.Subject;
    options.ClaimsIdentity.RoleClaimType = Claims.Role;
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = Schemes.Bearer;
    options.DefaultChallengeScheme = Schemes.Bearer;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(a=>a
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
    );
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
