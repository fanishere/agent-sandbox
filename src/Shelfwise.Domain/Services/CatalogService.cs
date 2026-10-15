using Shelfwise.Domain.Errors;
using Shelfwise.Domain.Models;
using Shelfwise.Domain.Repositories;

namespace Shelfwise.Domain.Services;

public sealed class CatalogService
{
    private readonly IBookRepository _books;

    public CatalogService(IBookRepository books)
    {
        _books = books;
    }

    public async Task<Book> GetAsync(Guid id)
    {
        return await _books.GetAsync(id)
            ?? throw new NotFoundException($"Book {id} does not exist.");
    }

    public Task<IReadOnlyList<Book>> ListAsync() => _books.ListAsync();

    public async Task<Book> AddAsync(CreateBookRequest request)
    {
        var book = new Book
        {
            Isbn = request.Isbn,
            Title = request.Title,
            Author = request.Author,
            Genre = request.Genre,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            AddedAt = DateTime.Now,
        };

        return await _books.AddAsync(book);
    }
}
