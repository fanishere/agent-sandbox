using System.Text.Json.Serialization;
using Shelfwise.Domain.Errors;
using Shelfwise.Domain.Models;
using Shelfwise.Domain.Repositories;
using Shelfwise.Domain.Services;
using Shelfwise.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddSingleton<IBookRepository, InMemoryBookRepository>();
builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
builder.Services.AddSingleton<PricingService>();
builder.Services.AddSingleton<InventoryService>();
builder.Services.AddSingleton<CatalogService>();
builder.Services.AddSingleton<OrderService>();

var app = builder.Build();

// Translate domain exceptions into HTTP problem responses.
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (NotFoundException ex)
    {
        await WriteProblem(context, StatusCodes.Status404NotFound, "Not found", ex.Message);
    }
    catch (InsufficientStockException ex)
    {
        await WriteProblem(context, StatusCodes.Status409Conflict, "Insufficient stock", ex.Message);
    }
    catch (DomainException ex)
    {
        await WriteProblem(context, StatusCodes.Status400BadRequest, "Business rule violated", ex.Message);
    }
});

// Demo data is only loaded for `dotnet run` (Development); tests run as "Testing" and start empty.
if (app.Environment.IsDevelopment())
{
    var bookRepository = app.Services.GetRequiredService<IBookRepository>();
    foreach (var book in SeedData.CreateBooks())
    {
        await bookRepository.AddAsync(book);
    }
}

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

// ---- Catalog ----------------------------------------------------------------

app.MapGet("/books", async (CatalogService catalog) =>
    Results.Ok(await catalog.ListAsync()));

app.MapGet("/books/{id:guid}", async (Guid id, CatalogService catalog) =>
    Results.Ok(await catalog.GetAsync(id)
        ?? throw new NotFoundException($"Book {id} was not found.")));

app.MapPost("/books", async (CreateBookRequest request, CatalogService catalog) =>
{
    var book = await catalog.AddAsync(request);
    return Results.Created($"/books/{book.Id}", book);
});

// ---- Orders -----------------------------------------------------------------

app.MapPost("/orders", async (PlaceOrderRequest request, OrderService orders) =>
{
    var order = await orders.PlaceOrderAsync(request);
    return Results.Created($"/orders/{order.Id}", order);
});

app.MapGet("/orders/{id:guid}", async (Guid id, OrderService orders) =>
    Results.Ok(await orders.GetAsync(id)));

app.MapPost("/orders/{id:guid}/pay", async (Guid id, OrderService orders) =>
    Results.Ok(await orders.MarkPaidAsync(id)));

app.MapPost("/orders/{id:guid}/ship", async (Guid id, OrderService orders) =>
    Results.Ok(await orders.ShipAsync(id)));

app.MapPost("/orders/{id:guid}/cancel", async (Guid id, OrderService orders) =>
    Results.Ok(await orders.CancelAsync(id)));

app.Run();

static Task WriteProblem(HttpContext context, int status, string title, string detail) =>
    Results.Problem(detail: detail, title: title, statusCode: status).ExecuteAsync(context);

// Required so WebApplicationFactory<Program> can see the entry point from tests.
public partial class Program
{
}
