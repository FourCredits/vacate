using Microsoft.EntityFrameworkCore;
using Vacate.Endpoints;
using Vacate.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddKeyPerFile("/run/secrets", optional: true);

var password = builder.Configuration.GetValue<string>("DatabasePassword");
var connectionString =
    $"Host=localhost;Database=vacate;Username=postgres;Password={password}";
builder.Services.AddDbContext<VacateContext>(options =>
    options.UseNpgsql(connectionString)
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPersonEndpoints();
app.MapProjectEndpoints();
app.MapAssignmentEndpoints();

app.Run();
