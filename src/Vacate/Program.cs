using Microsoft.EntityFrameworkCore;
using Vacate.Endpoints;
using Vacate.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddKeyPerFile("/run/secrets", optional: true);
builder.Configuration.AddEnvironmentVariables();

var host = builder.Configuration.GetValue<string>("Database:Host", "localhost");
var database = builder.Configuration.GetValue<string>("Database:Database", "vacate");
var username = builder.Configuration.GetValue<string>("Database:Username", "postgres");
var password = builder.Configuration.GetValue<string>("Database:Password");
var connectionString =
    $"Host={host};Database={database};Username={username};Password={password}";
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

app.MapPersonEndpoints();
app.MapProjectEndpoints();
app.MapAssignmentEndpoints();

app.Run();
