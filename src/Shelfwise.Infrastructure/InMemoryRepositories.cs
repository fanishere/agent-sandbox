using System.Collections.Concurrent;
using Shelfwise.Domain.Models;
using Shelfwise.Domain.Repositories;

namespace Shelfwise.Infrastructure;

public sealed class InMemoryBookRepository : IBookRepository
{
    private readonly ConcurrentDictionary<Guid, Book> _books = new();

    public Task<Book?> GetAsync(Guid id) =>
        Task.FromResult<Book?>(_books.TryGetValue(id, out var book) ? book : null);

    public Task<IReadOnlyList<Book>> ListAsync() =>
        Task.FromResult<IReadOnlyList<Book>>(_books.Values.OrderBy(b => b.Title).ToList());

    public Task<Book> AddAsync(Book book)
    {
        _books[book.Id] = book;
        return Task.FromResult(book);
    }

    public Task UpdateAsync(Book book)
    {
        _books[book.Id] = book;
        return Task.CompletedTask;
    }
}

public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<Guid, Order> _orders = new();

    public Task<Order?> GetAsync(Guid id) =>
        Task.FromResult<Order?>(_orders.TryGetValue(id, out var order) ? order : null);

    public Task<IReadOnlyList<Order>> ListAsync() =>
        Task.FromResult<IReadOnlyList<Order>>(_orders.Values.OrderBy(o => o.CreatedAt).ToList());

    public Task<Order> AddAsync(Order order)
    {
        _orders[order.Id] = order;
        return Task.FromResult(order);
    }

    public Task UpdateAsync(Order order)
    {
        _orders[order.Id] = order;
        return Task.CompletedTask;
    }
}
