using Shelfwise.Domain.Errors;
using Shelfwise.Domain.Repositories;

namespace Shelfwise.Domain.Services;

public sealed class InventoryService
{
    private readonly IBookRepository _books;

    public InventoryService(IBookRepository books)
    {
        _books = books;
    }

    /// <summary>Takes <paramref name="quantity"/> copies off the shelf.</summary>
    public async Task ReserveAsync(Guid bookId, int quantity)
    {
        var book = await _books.GetAsync(bookId)
            ?? throw new NotFoundException($"Book {bookId} was not found.");

        if (book.StockQuantity < quantity)
        {
            throw new InsufficientStockException(book.Id, quantity, book.StockQuantity);
        }

        book.StockQuantity -= quantity;
        await _books.UpdateAsync(book);
    }

    /// <summary>Puts <paramref name="quantity"/> copies back on the shelf.</summary>
    public async Task ReleaseAsync(Guid bookId, int quantity)
    {
        var book = await _books.GetAsync(bookId)
            ?? throw new NotFoundException($"Book {bookId} was not found.");

        book.StockQuantity += quantity;
        await _books.UpdateAsync(book);
    }
}
