using Microsoft.EntityFrameworkCore;
using Vacate.Models;

namespace Vacate.Persistence;

public class VacateContext(DbContextOptions<VacateContext> options)
    : DbContext(options)
{
    public DbSet<Person> People { get; set; } = default!;
    public DbSet<Project> Projects { get; set; } = default!;
    public DbSet<Assignment> Assignments { get; set; } = default!;
}
