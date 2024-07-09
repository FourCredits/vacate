namespace Vacate.Models;

public class Person
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int HoursWorkedPerWeek { get; set; }
    public ICollection<Assignment> Assignments { get; set; } = [];
}
