namespace Vacate.Models;

public class Assignment
{
    public int Id { get; set; }
    public int PersonId { get; set; }
    public Person Person { get; set; } = default!;
    public int ProjectId { get; set; }
    public Project Project { get; set; } = default!;
    public int HoursWorkedPerWeek { get; set; }
}
