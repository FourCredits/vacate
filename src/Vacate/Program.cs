using Microsoft.EntityFrameworkCore;
using Vacate.Endpoints;
using Vacate.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddKeyPerFile("/run/secrets", optional: true);
builder.Configuration.AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString();
builder.Services.AddVacateDbContext(connectionString);
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
