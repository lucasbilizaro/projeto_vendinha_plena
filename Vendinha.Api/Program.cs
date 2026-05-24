using Microsoft.EntityFrameworkCore;
using Vendinha.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Banco SQLite
builder.Services.AddDbContext<VendinhaDbContext>(options =>
    options.UseSqlite("Data Source=vendinha.db"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();