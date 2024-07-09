using Microsoft.EntityFrameworkCore;
using Vacate.Endpoints;
using Vacate.Persistence;
using Vacate;

var builder = WebApplication.CreateBuilder(args);

// TODO: don't hardcode connection string
builder.Services.AddDbContext<VacateContext>(
    options =>
        options.UseNpgsql(
            "Host=localhost;Database=vacate;Username=postgres;Password=postgres"
        )
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
