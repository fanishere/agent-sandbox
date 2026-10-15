using Shelfwise.Domain.Errors;
using Shelfwise.Domain.Models;
using Shelfwise.Domain.Repositories;

namespace Shelfwise.Domain.Services;

public sealed class OrderService
{
    private readonly IBookRepository _books;
    private readonly IOrderRepository _orders;
    private readonly PricingService _pricing;
    private readonly InventoryService _inventory;

    public OrderService(
        IBookRepository books,
        IOrderRepository orders,
        PricingService pricing,
        InventoryService inventory)
    {
        _books = books;
        _orders = orders;
        _pricing = pricing;
        _inventory = inventory;
    }

    public async Task<Order> PlaceOrderAsync(PlaceOrderRequest request)
    {
        if (request.Lines.Count == 0)
        {
            throw new DomainException("An order needs at least one line.");
        }

        var order = new Order
        {
            CustomerEmail = request.CustomerEmail,
            CreatedAt = DateTime.UtcNow,
        };

        var reserved = new List<(Guid BookId, int Quantity)>();
        try
        {
            foreach (var lineRequest in request.Lines)
            {
                var book = await _books.GetAsync(lineRequest.BookId)
                    ?? throw new NotFoundException($"Book {lineRequest.BookId} was not found.");

                await _inventory.ReserveAsync(book.Id, lineRequest.Quantity);
                reserved.Add((book.Id, lineRequest.Quantity));

                order.Lines.Add(new OrderLine
                {
                    BookId = book.Id,
                    Title = book.Title,
                    Quantity = lineRequest.Quantity,
                    UnitPrice = book.Price,
                    LineTotal = _pricing.PriceLine(book.Price, lineRequest.Quantity),
                });
            }
        }
        catch
        {
            // Placement is all-or-nothing: give back whatever earlier lines took.
            foreach (var (bookId, quantity) in reserved)
            {
                await _inventory.ReleaseAsync(bookId, quantity);
            }

            throw;
        }

        order.Total = order.Lines.Sum(l => l.LineTotal);

        return await _orders.AddAsync(order);
    }

    public async Task<Order> GetAsync(Guid id)
    {
        return await _orders.GetAsync(id)
            ?? throw new NotFoundException($"Order {id} was not found.");
    }

    public async Task<Order> MarkPaidAsync(Guid id)
    {
        var order = await GetAsync(id);
        order.Status = OrderStatus.Paid;
        await _orders.UpdateAsync(order);
        return order;
    }

    public async Task<Order> ShipAsync(Guid id)
    {
        var order = await GetAsync(id);
        order.Status = OrderStatus.Shipped;
        await _orders.UpdateAsync(order);
        return order;
    }

    public async Task<Order> CancelAsync(Guid id)
    {
        var order = await GetAsync(id);
        order.Status = OrderStatus.Cancelled;
        await _orders.UpdateAsync(order);
        return order;
    }
}
