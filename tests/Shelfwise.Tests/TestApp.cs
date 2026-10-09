using Shelfwise.Domain.Models;
using Shelfwise.Domain.Services;
using Shelfwise.Infrastructure;

namespace Shelfwise.Tests;

/// <summary>Wires up the domain services over in-memory repositories for unit tests.</summary>
internal sealed class TestApp
{
    private static int _isbnCounter;

    public InMemoryBookRepository Books { get; } = new();
    public InMemoryOrderRepository Orders { get; } = new();
    public PricingService Pricing { get; } = new();
    public InventoryService Inventory { get; }
    public CatalogService Catalog { get; }
    public OrderService OrderService { get; }

    public TestApp()
    {
        Inventory = new InventoryService(Books);
        Catalog = new CatalogService(Books);
        OrderService = new OrderService(Books, Orders, Pricing, Inventory);
    }

    public Task<Book> AddBookAsync(decimal price = 10m, int stock = 5, string title = "Test Book")
    {
        var n = Interlocked.Increment(ref _isbnCounter);

        return Books.AddAsync(new Book
        {
            Isbn = $"978000000{n:D4}",
            Title = title,
            Author = "Test Author",
            Genre = "Testing",
            Price = price,
            StockQuantity = stock,
            AddedAt = DateTime.UtcNow,
        });
    }
}
