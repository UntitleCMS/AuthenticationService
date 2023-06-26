using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using AuthenticationService.Datas;
using AuthenticationService;
using System;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Abstractions;
using AuthenticationService.Authentication.Register;
using AuthenticationService.Authentication.Token;
using AuthenticationService.Authentication.Profile;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddControllers()

    // Add Json Handeler
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore; 
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthenticationService", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

// Add CORS
builder.Services.AddCors();

// Get connection string and Assembly Name
string migrationsAssembly = typeof(Program).Assembly.GetName().Name ?? string.Empty;
string connectionString
    = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
    ?? builder.Configuration.GetConnectionString("default")
    ?? throw new InvalidOperationException("Connection string not found!");

// Add Database Context
builder.Services.AddDbContext<AppDbContext>(opt => opt
    .UseSqlServer(connectionString)
    .UseOpenIddict()
);

// Add OpendIddict configuration
builder.Services.AddMyOpendIddictConfiguration();

// Add ASP.NET Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddSignInManager()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddUserManager<UserManager<IdentityUser>>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
    options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
    options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = OpenIddictConstants.Schemes.Bearer;
    options.DefaultChallengeScheme = OpenIddictConstants.Schemes.Bearer;
});



builder.Services.AddScoped<RegisterService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<ProfileService>();


builder.Services.BuildServiceProvider()
    .GetService<AppDbContext>()!
    .Database.EnsureCreated();

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

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
