using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Vacate.Models;
using Vacate.Persistence;

namespace Vacate.Endpoints;

public static class AssignmentEndpoints
{
    public static void MapAssignmentEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes
            .MapGroup("/api/assignment")
            .WithTags(nameof(Assignment));

        group
            .MapGet("/", GetAllAssignments)
            .WithName("GetAllAssignments")
            .WithOpenApi();

        group
            .MapGet("/by-project/{id}", GetAssignmentsByProjectId)
            .WithName("GetAssignmentsByProjectId")
            .WithOpenApi();

        group
            .MapGet("/by-person/{id}", GetAssignmentsByPersonId)
            .WithName("GetAssignmentsByPersonId")
            .WithOpenApi();

        group
            .MapGet("/{id}", GetAssignmentById)
            .WithName("GetAssignmentById")
            .WithOpenApi();

        group
            .MapPut("/{id}", UpdateAssignment)
            .WithName("UpdateAssignment")
            .WithOpenApi();

        group
            .MapPost("/", CreateAssignment)
            .WithName("CreateAssignment")
            .WithOpenApi();

        group
            .MapDelete("/{id}", DeleteAssignment)
            .WithName("DeleteAssignment")
            .WithOpenApi();
    }

    private static async Task<List<Assignment>> GetAllAssignments(
        VacateContext db
    ) => await db.Assignments.ToListAsync();

    private static async Task<List<Assignment>> GetAssignmentsByProjectId(
        int projectId,
        VacateContext db
    ) =>
        await db.Assignments.Where(a => a.ProjectId == projectId).ToListAsync();

    private static async Task<List<Assignment>> GetAssignmentsByPersonId(
        int personId,
        VacateContext db
    ) => await db.Assignments.Where(a => a.PersonId == personId).ToListAsync();

    private static async Task<
        Results<Ok<Assignment>, NotFound>
    > GetAssignmentById(int id, VacateContext db) =>
        await db.Assignments
            .AsNoTracking()
            .FirstOrDefaultAsync(model => model.Id == id)
            is Assignment model
            ? TypedResults.Ok(model)
            : TypedResults.NotFound();

    private static async Task<Results<Ok, NotFound>> UpdateAssignment(
        int id,
        UpdateAssignmentRequest updateRequest,
        VacateContext db
    )
    {
        var affected = await db.Assignments
            .Where(model => model.Id == id)
            .ExecuteUpdateAsync(
                setters =>
                    setters
                        .SetProperty(m => m.PersonId, updateRequest.PersonId)
                        .SetProperty(m => m.ProjectId, updateRequest.ProjectId)
                        .SetProperty(
                            m => m.HoursWorkedPerWeek,
                            updateRequest.HoursWorkedPerWeek
                        )
            );
        return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
    }

    private record UpdateAssignmentRequest(
        int PersonId,
        int ProjectId,
        int HoursWorkedPerWeek
    );

    private static async Task<Created<Assignment>> CreateAssignment(
        CreateAssignmentRequest createRequest,
        VacateContext db
    )
    {
        var assignment = createRequest.ToAssignment();
        db.Assignments.Add(assignment);
        await db.SaveChangesAsync();
        return TypedResults.Created(
            $"/api/Assignment/{assignment.Id}",
            assignment
        );
    }

    private record CreateAssignmentRequest(
        int PersonId,
        int ProjectId,
        int HoursWorkedPerWeek
    )
    {
        public Assignment ToAssignment() =>
            new Assignment
            {
                PersonId = PersonId,
                ProjectId = ProjectId,
                HoursWorkedPerWeek = HoursWorkedPerWeek,
            };
    }

    private static async Task<Results<Ok, NotFound>> DeleteAssignment(
        int id,
        VacateContext db
    )
    {
        var affected = await db.Assignments
            .Where(model => model.Id == id)
            .ExecuteDeleteAsync();
        return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
    }
}
