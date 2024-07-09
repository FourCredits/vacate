using Vacate.Models;
using Vacate.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Vacate.Endpoints;

public static class PeopleEndpoints
{
    public static void MapPersonEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/person").WithTags(nameof(Person));

        group.MapGet("/", GetAllPeople).WithName("GetAllPeople").WithOpenApi();

        group
            .MapGet("/{id}", GetPersonById)
            .WithName("GetPersonById")
            .WithOpenApi();

        group
            .MapPut("/{id}", UpdatePerson)
            .WithName("UpdatePerson")
            .WithOpenApi();

        group.MapPost("/", CreatePerson).WithName("CreatePerson").WithOpenApi();

        group
            .MapDelete("/{id}", DeletePerson)
            .WithName("DeletePerson")
            .WithOpenApi();
    }

    private static Task<List<Person>> GetAllPeople(VacateContext db) =>
        db.People.ToListAsync();

    private static async Task<Results<Ok<Person>, NotFound>> GetPersonById(
        int id,
        VacateContext db
    ) =>
        await db.People
            .AsNoTracking()
            .FirstOrDefaultAsync(model => model.Id == id)
            is Person model
            ? TypedResults.Ok(model)
            : TypedResults.NotFound();

    private static async Task<Results<Ok, NotFound>> UpdatePerson(
        int id,
        UpdatePersonRequest updateRequest,
        VacateContext db
    )
    {
        var affected = await db.People
            .Where(model => model.Id == id)
            .ExecuteUpdateAsync(
                setters =>
                    setters
                        .SetProperty(m => m.Name, updateRequest.Name)
                        .SetProperty(
                            m => m.HoursWorkedPerWeek,
                            updateRequest.HoursWorkedPerWeek
                        )
            );
        return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
    }

    private record UpdatePersonRequest(string Name, int HoursWorkedPerWeek);

    private static async Task<Created<Person>> CreatePerson(
        CreatePersonRequest createRequest,
        VacateContext db
    )
    {
        var person = createRequest.ToPerson();
        db.People.Add(person);
        await db.SaveChangesAsync();
        return TypedResults.Created($"/api/Person/{person.Id}", person);
    }

    private record CreatePersonRequest(string Name, int HoursWorkedPerWeek)
    {
        public Person ToPerson() =>
            new Person { Name = Name, HoursWorkedPerWeek = HoursWorkedPerWeek };
    }

    private static async Task<Results<Ok, NotFound>> DeletePerson(
        int id,
        VacateContext db
    )
    {
        var affected = await db.People
            .Where(model => model.Id == id)
            .ExecuteDeleteAsync();
        return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
    }
}
