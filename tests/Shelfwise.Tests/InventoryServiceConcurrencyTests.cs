using Shelfwise.Domain.Errors;
using Shelfwise.Domain.Models;
using Shelfwise.Domain.Repositories;
using Shelfwise.Domain.Services;

namespace Shelfwise.Tests;

/// <summary>
/// Deterministic reproduction of the SHW-106 oversell. The wrapping repository
/// hands out a *copy* of the book on read (like a SQL row materialised into
/// memory) and parks the first stock read for a given book until the test
/// releases it, forcing a second concurrent reserve to interleave between the
/// first caller's check and its write.
/// </summary>
public class InventoryServiceConcurrencyTests
{
    private readonly TestApp _app = new();

    [Fact]
    public async Task Reserve_ConcurrentOrdersForSameBook_NeverOversells()
    {
        const int initialStock = 3;
        const int quantityEach = 2; // two orders of 2; only one can fit in stock 3
        var book = await _app.AddBookAsync(stock: initialStock);
        var gate = new InterleavingBookRepository(_app.Books);
        var inventory = new InventoryService(gate);

        var first = inventory.ReserveAsync(book.Id, quantityEach);
        await gate.FirstReadArrived; // first caller has read stock 3, is parked before the check

        var second = inventory.ReserveAsync(book.Id, quantityEach);
        await Task.Delay(100); // second caller runs to its read (pre-fix) or blocks on the per-book lock (post-fix)

        gate.ReleaseFirstRead();
        var firstOk = await Succeeded(first);
        var secondOk = await Succeeded(second);

        var succeeded = (firstOk ? 1 : 0) + (secondOk ? 1 : 0);
        var finalStock = (await _app.Books.GetAsync(book.Id))!.StockQuantity;
        var totalSold = succeeded * quantityEach;

        Assert.True(totalSold <= initialStock,
            $"oversold: {totalSold} copies confirmed from stock {initialStock} (final stock {finalStock})");
        Assert.True(finalStock >= 0, $"stock went negative: {finalStock}");
        Assert.Equal(initialStock - totalSold, finalStock);
    }

    [Fact]
    public async Task Reserve_DifferentBooks_AreNotSerializedBehindOneLock()
    {
        var bookA = await _app.AddBookAsync(stock: 5, title: "A");
        var bookB = await _app.AddBookAsync(stock: 5, title: "B");
        var gate = new InterleavingBookRepository(_app.Books);
        var inventory = new InventoryService(gate);

        var reserveA = inventory.ReserveAsync(bookA.Id, 1);
        await gate.FirstReadArrived; // book A's reserve is parked mid-flight

        // If reserves were behind a single global lock, this would deadlock
        // until the timeout because book B could never acquire the lock.
        var reserveB = inventory.ReserveAsync(bookB.Id, 1);
        var finished = await Task.WhenAny(reserveB, Task.Delay(TimeSpan.FromSeconds(5)));

        gate.ReleaseFirstRead();
        await Task.WhenAll(reserveA, reserveB);

        Assert.Same(reserveB, finished);
    }

    private static async Task<bool> Succeeded(Task task)
    {
        try
        {
            await task;
            return true;
        }
        catch (InsufficientStockException)
        {
            return false;
        }
    }

    /// <summary>
    /// Wraps a repository: reads return a copy of the stored book and writes
    /// store the given instance (so lost updates are observable, as with a SQL
    /// row), and the first <see cref="GetAsync"/> for a book is parked until
    /// the test releases it.
    /// </summary>
    private sealed class InterleavingBookRepository : IBookRepository
    {
        private readonly IBookRepository _inner;
        private readonly TaskCompletionSource _firstReadArrived =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly TaskCompletionSource _releaseFirstRead =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
        private int _readCount;

        public InterleavingBookRepository(IBookRepository inner) => _inner = inner;

        public Task FirstReadArrived => _firstReadArrived.Task;
        public void ReleaseFirstRead() => _releaseFirstRead.TrySetResult();

        public async Task<Book?> GetAsync(Guid id)
        {
            var book = await _inner.GetAsync(id);
            if (book is null)
            {
                return null;
            }

            if (Interlocked.Increment(ref _readCount) == 1)
            {
                _firstReadArrived.TrySetResult();
                await _releaseFirstRead.Task;
            }

            return Clone(book);
        }

        public Task<IReadOnlyList<Book>> ListAsync() => _inner.ListAsync();
        public Task<Book> AddAsync(Book book) => _inner.AddAsync(book);
        public Task UpdateAsync(Book book) => _inner.UpdateAsync(book);

        private static Book Clone(Book book) => new()
        {
            Id = book.Id,
            Isbn = book.Isbn,
            Title = book.Title,
            Author = book.Author,
            Genre = book.Genre,
            Price = book.Price,
            StockQuantity = book.StockQuantity,
            AddedAt = book.AddedAt,
        };
    }
}
