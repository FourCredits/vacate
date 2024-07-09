using Vacate.Models;
using Vacate.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Vacate.Endpoints;

public static class ProjectEndpoints
{
    public static void MapProjectEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/project").WithTags(nameof(Project));

        group
            .MapGet("/", GetAllProjects)
            .WithName("GetAllProjects")
            .WithOpenApi();

        group
            .MapGet("/{id}", GetProjectById)
            .WithName("GetProjectById")
            .WithOpenApi();

        group
            .MapPut("/{id}", UpdateProject)
            .WithName("UpdateProject")
            .WithOpenApi();

        group
            .MapPost("/", CreateProject)
            .WithName("CreateProject")
            .WithOpenApi();

        group
            .MapDelete("/{id}", DeleteProject)
            .WithName("DeleteProject")
            .WithOpenApi();
    }

    private static async Task<List<Project>> GetAllProjects(VacateContext db) =>
        await db.Projects.ToListAsync();

    private static async Task<Results<Ok<Project>, NotFound>> GetProjectById(
        int id,
        VacateContext db
    ) =>
        await db.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(model => model.Id == id)
            is Project model
            ? TypedResults.Ok(model)
            : TypedResults.NotFound();

    private static async Task<Results<Ok, NotFound>> UpdateProject(
        int id,
        UpdateProjectRequest updateRequest,
        VacateContext db
    )
    {
        var affected = await db.Projects
            .Where(model => model.Id == id)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(m => m.Name, updateRequest.Name)
            );
        return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
    }

    private record UpdateProjectRequest(string Name);

    private static async Task<Created<Project>> CreateProject(
        CreateProjectRequest createRequest,
        VacateContext db
    )
    {
        var project = createRequest.ToProject();
        db.Projects.Add(project);
        await db.SaveChangesAsync();
        return TypedResults.Created($"/api/Project/{project.Id}", project);
    }

    private record CreateProjectRequest(string Name)
    {
        public Project ToProject() => new Project { Name = Name };
    }

    private static async Task<Results<Ok, NotFound>> DeleteProject(
        int id,
        VacateContext db
    )
    {
        var affected = await db.Projects
            .Where(model => model.Id == id)
            .ExecuteDeleteAsync();
        return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
    }
}
