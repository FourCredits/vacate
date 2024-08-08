using Microsoft.EntityFrameworkCore;

namespace Vacate.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddVacateDbContext(
        this IServiceCollection services,
        string connectionString
    ) =>
        services.AddDbContext<VacateContext>(options =>
            options.UseNpgsql(connectionString)
        );

    public static string GetConnectionString(this IConfiguration configuration)
    {
        var host = configuration.GetValue<string>("Database:Host", "localhost");
        var database = configuration.GetValue<string>(
            "Database:Database",
            "vacate"
        );
        var username = configuration.GetValue<string>(
            "Database:Username",
            "postgres"
        );
        var password = configuration.GetValue<string>("Database:Password");
        return $"Host={host};Database={database};Username={username};Password={password}";
    }
}
