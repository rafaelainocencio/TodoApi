using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

//Add DI (dependency injection) - AddService
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));

var app = builder.Build();

// Configure pipeline UseMethod...
app.MapGet("/todoitems", async (TodoDb db) =>
{
    var items = await db.TodoItems.ToListAsync();
    return Results.Ok(items);
});

app.MapGet("/todoitems/{id}", async (TodoDb db, int id) =>
{
    var item = await db.TodoItems.FindAsync(id);
    if (item == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(item);
});

app.MapPost("/todoitems", async (TodoItem item, TodoDb db) =>
{
    db.TodoItems.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/todoitems/{item.Id}", item);
});

app.MapPut("/todoitems/{id}", async (TodoItem item, TodoDb db, int id) =>
{
    if (id != item.Id)
    {
        return Results.BadRequest();
    }

    db.Entry(item).State = EntityState.Modified;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/todoitems/{id}", async (TodoDb db, int id) =>
{
    var item = await db.TodoItems.FindAsync(id);
    if (item == null)
    {
        return Results.NotFound();
    }

    db.TodoItems.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
