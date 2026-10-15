using System.Collections.Concurrent;
using Shelfwise.Domain.Errors;
using Shelfwise.Domain.Repositories;

namespace Shelfwise.Domain.Services;

public sealed class InventoryService
{
    private readonly IBookRepository _books;

    // One lock per book so the check-then-write sequence is atomic for a given
    // title without serialising inventory operations across different books.
    private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _bookLocks = new();

    public InventoryService(IBookRepository books)
    {
        _books = books;
    }

    /// <summary>Takes <paramref name="quantity"/> copies off the shelf.</summary>
    public async Task ReserveAsync(Guid bookId, int quantity)
    {
        await RunExclusivelyAsync(bookId, async () =>
        {
            var book = await _books.GetAsync(bookId)
                ?? throw new NotFoundException($"Book {bookId} was not found.");

            if (book.StockQuantity < quantity)
            {
                throw new InsufficientStockException(book.Id, quantity, book.StockQuantity);
            }

            book.StockQuantity -= quantity;
            await _books.UpdateAsync(book);
        });
    }

    /// <summary>Puts <paramref name="quantity"/> copies back on the shelf.</summary>
    public async Task ReleaseAsync(Guid bookId, int quantity)
    {
        await RunExclusivelyAsync(bookId, async () =>
        {
            var book = await _books.GetAsync(bookId)
                ?? throw new NotFoundException($"Book {bookId} was not found.");

            book.StockQuantity += quantity;
            await _books.UpdateAsync(book);
        });
    }

    private async Task RunExclusivelyAsync(Guid bookId, Func<Task> action)
    {
        var bookLock = _bookLocks.GetOrAdd(bookId, _ => new SemaphoreSlim(1, 1));
        await bookLock.WaitAsync();
        try
        {
            await action();
        }
        finally
        {
            bookLock.Release();
        }
    }
}
