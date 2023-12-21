using Microsoft.EntityFrameworkCore;
using AuthenticationService.Datas;
using AuthenticationService;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using Microsoft.OpenApi.Models;
using AuthenticationService.Entitis;
using AuthenticationService.Features.Login;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddControllers()

    // Add Json Handeler
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });
builder.Services.AddRazorPages();

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


builder.Services.AddScoped<UsernameLoginService>();


builder.Services.BuildServiceProvider()
    .GetService<AppDbContext>()!
    .Database.EnsureCreated();

var app = builder.Build();

app.UsePathBase("/api/auth/v2");

app.UseForwardedHeaders(new() { ForwardedHeaders = ForwardedHeaders.XForwardedProto });
app.UseForwardedHeaders(new() { ForwardedHeaders = ForwardedHeaders.XForwardedHost });

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(a => a
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
    );
}

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();
