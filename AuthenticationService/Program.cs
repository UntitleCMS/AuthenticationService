using DataAccess.Datas;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors();

// Add DB
string connectionString = "Server=database;Database=authentication;User=sa;Password=P@ssword;TrustServerCertificate=True;";
var migrationsAssembly = typeof(Program).Assembly.GetName().Name;

builder.Services.AddDbContext<TmpDataContext>(opt => opt
    .UseSqlServer(connectionString)
);

builder.Services.AddDbContext<AspNetIdentityDbContext>(opt => opt
    .UseSqlServer(connectionString, opt => opt.MigrationsAssembly(migrationsAssembly))
);


// Add Identity
builder.Services
    .AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AspNetIdentityDbContext>();

//builder.Services
//    .AddIdentityServer()
//    .AddAspNetIdentity<AspNetIdentityDbContext>()
//    .AddConfigurationStore(opt => opt.ConfigureDbContext =
//            b => b.UseSqlServer(connectionString, opt => opt.MigrationsAssembly(migrationsAssembly)))
//    .AddOperationalStore(opt => opt.ConfigureDbContext =
//            b => b.UseSqlServer(connectionString, opt => opt.MigrationsAssembly(migrationsAssembly)))
//    .AddDeveloperSigningCredential();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(a=>a
        .AllowAnyOrigin()
        //.AllowCredentials()
        .AllowAnyHeader()
        .AllowAnyMethod()
    );
}

app.UseHttpsRedirection();

//app.UseIdentityServer();

app.UseAuthorization();

app.MapControllers();

app.Run();
