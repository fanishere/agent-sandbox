using Shelfwise.Domain.Models;

namespace Shelfwise.Domain.Repositories;

public interface IBookRepository
{
    Task<Book?> GetAsync(Guid id);
    Task<IReadOnlyList<Book>> ListAsync();
    Task<Book> AddAsync(Book book);
    Task UpdateAsync(Book book);
}

public interface IOrderRepository
{
    Task<Order?> GetAsync(Guid id);
    Task<IReadOnlyList<Order>> ListAsync();
    Task<Order> AddAsync(Order order);
    Task UpdateAsync(Order order);
}
