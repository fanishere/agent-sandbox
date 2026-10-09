using Shelfwise.Domain.Models;

namespace Shelfwise.Infrastructure;

public static class SeedData
{
    public static IReadOnlyList<Book> CreateBooks()
    {
        var addedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        return new List<Book>
        {
            new() { Isbn = "9780132350884", Title = "Clean Code", Author = "Robert C. Martin", Genre = "Programming", Price = 37.99m, StockQuantity = 25, AddedAt = addedAt },
            new() { Isbn = "9780134494166", Title = "Clean Architecture", Author = "Robert C. Martin", Genre = "Programming", Price = 34.50m, StockQuantity = 12, AddedAt = addedAt },
            new() { Isbn = "9780135957059", Title = "The Pragmatic Programmer", Author = "David Thomas, Andrew Hunt", Genre = "Programming", Price = 41.95m, StockQuantity = 30, AddedAt = addedAt },
            new() { Isbn = "9780134757599", Title = "Refactoring", Author = "Martin Fowler", Genre = "Programming", Price = 5.15m, StockQuantity = 50, AddedAt = addedAt },
            new() { Isbn = "9780321125217", Title = "Domain-Driven Design", Author = "Eric Evans", Genre = "Architecture", Price = 59.99m, StockQuantity = 3, AddedAt = addedAt },
            new() { Isbn = "9781449373320", Title = "Designing Data-Intensive Applications", Author = "Martin Kleppmann", Genre = "Architecture", Price = 49.99m, StockQuantity = 0, AddedAt = addedAt },
            new() { Isbn = "9780596007126", Title = "Head First Design Patterns", Author = "Eric Freeman, Elisabeth Robson", Genre = "Programming", Price = 44.00m, StockQuantity = 8, AddedAt = addedAt },
            new() { Isbn = "9781491950357", Title = "Building Microservices", Author = "Sam Newman", Genre = "Architecture", Price = 47.25m, StockQuantity = 15, AddedAt = addedAt },
        };
    }
}
