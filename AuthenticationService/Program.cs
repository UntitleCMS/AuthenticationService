using DataAccess.Datas;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors();

// Ad DB
string connectionString = "Server=database;Database=auth;User=sa;Password=P@ssword;TrustServerCertificate=True;";
builder.Services.AddDbContext<TmpDataContext>(opt => opt
    .UseSqlServer(connectionString)
);

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

app.UseAuthorization();

app.MapControllers();

app.Run();
