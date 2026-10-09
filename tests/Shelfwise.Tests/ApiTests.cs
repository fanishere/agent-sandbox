using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Shelfwise.Tests;

/// <summary>Boots the real API in-process with an empty catalog.</summary>
public sealed class ShelfwiseFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
    }
}

public class ApiTests : IClassFixture<ShelfwiseFactory>
{
    private readonly HttpClient _client;

    public ApiTests(ShelfwiseFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_ReturnsOk()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateBook_ThenGetById_RoundTrips()
    {
        var created = await CreateBookAsync("Round Trip", price: 15.00m, stock: 4);

        var response = await _client.GetAsync($"/books/{created}");
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Round Trip", json.GetProperty("title").GetString());
    }

    [Fact]
    public async Task PlaceOrder_ThenGetById_ReturnsPendingOrder()
    {
        var bookId = await CreateBookAsync("Orderable", price: 10.00m, stock: 10);

        var placed = await _client.PostAsJsonAsync("/orders", new
        {
            customerEmail = "reader@example.com",
            lines = new[] { new { bookId, quantity = 2 } },
        });
        Assert.Equal(HttpStatusCode.Created, placed.StatusCode);

        var order = await placed.Content.ReadFromJsonAsync<JsonElement>();
        var orderId = order.GetProperty("id").GetGuid();

        var fetched = await _client.GetAsync($"/orders/{orderId}");
        var fetchedJson = await fetched.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.OK, fetched.StatusCode);
        Assert.Equal("Pending", fetchedJson.GetProperty("status").GetString());
        Assert.Equal(20.00m, fetchedJson.GetProperty("total").GetDecimal());
    }

    [Fact]
    public async Task GetOrder_Unknown_Returns404()
    {
        var response = await _client.GetAsync($"/orders/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PlaceOrder_MoreThanInStock_Returns409()
    {
        var bookId = await CreateBookAsync("Scarce", price: 10.00m, stock: 1);

        var response = await _client.PostAsJsonAsync("/orders", new
        {
            customerEmail = "reader@example.com",
            lines = new[] { new { bookId, quantity = 5 } },
        });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    private static int _isbnCounter;

    private async Task<Guid> CreateBookAsync(string title, decimal price, int stock)
    {
        var n = Interlocked.Increment(ref _isbnCounter);

        var response = await _client.PostAsJsonAsync("/books", new
        {
            isbn = $"978100000{n:D4}",
            title,
            author = "Test Author",
            genre = "Testing",
            price,
            stockQuantity = stock,
        });
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        return json.GetProperty("id").GetGuid();
    }
}
