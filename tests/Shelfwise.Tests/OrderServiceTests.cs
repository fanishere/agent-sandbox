using Shelfwise.Domain.Errors;
using Shelfwise.Domain.Models;

namespace Shelfwise.Tests;

public class OrderServiceTests
{
    private readonly TestApp _app = new();

    [Fact]
    public async Task PlaceOrder_ComputesTotalAndReducesStock()
    {
        var bookA = await _app.AddBookAsync(price: 10.00m, stock: 5, title: "A");
        var bookB = await _app.AddBookAsync(price: 20.00m, stock: 5, title: "B");

        var order = await _app.OrderService.PlaceOrderAsync(new PlaceOrderRequest(
            "reader@example.com",
            new List<OrderLineRequest>
            {
                new(bookA.Id, 2),
                new(bookB.Id, 1),
            }));

        Assert.Equal(40.00m, order.Total);
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Equal(2, order.Lines.Count);
        Assert.Equal(3, (await _app.Books.GetAsync(bookA.Id))!.StockQuantity);
        Assert.Equal(4, (await _app.Books.GetAsync(bookB.Id))!.StockQuantity);
    }

    [Fact]
    public async Task PlaceOrder_WithNoLines_Throws()
    {
        await Assert.ThrowsAsync<DomainException>(() =>
            _app.OrderService.PlaceOrderAsync(
                new PlaceOrderRequest("reader@example.com", new List<OrderLineRequest>())));
    }

    [Fact]
    public async Task PlaceOrder_UnknownBook_Throws()
    {
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _app.OrderService.PlaceOrderAsync(new PlaceOrderRequest(
                "reader@example.com",
                new List<OrderLineRequest> { new(Guid.NewGuid(), 1) })));
    }

    [Fact]
    public async Task PlaceOrder_MoreThanInStock_Throws()
    {
        var book = await _app.AddBookAsync(stock: 2);

        await Assert.ThrowsAsync<InsufficientStockException>(() =>
            _app.OrderService.PlaceOrderAsync(new PlaceOrderRequest(
                "reader@example.com",
                new List<OrderLineRequest> { new(book.Id, 3) })));
    }

    [Fact]
    public async Task MarkPaid_SetsStatusToPaid()
    {
        var book = await _app.AddBookAsync();
        var order = await _app.OrderService.PlaceOrderAsync(new PlaceOrderRequest(
            "reader@example.com",
            new List<OrderLineRequest> { new(book.Id, 1) }));

        var paid = await _app.OrderService.MarkPaidAsync(order.Id);

        Assert.Equal(OrderStatus.Paid, paid.Status);
    }

    [Fact]
    public async Task Cancel_PendingOrder_SetsStatusToCancelled()
    {
        var book = await _app.AddBookAsync();
        var order = await _app.OrderService.PlaceOrderAsync(new PlaceOrderRequest(
            "reader@example.com",
            new List<OrderLineRequest> { new(book.Id, 1) }));

        var cancelled = await _app.OrderService.CancelAsync(order.Id);

        Assert.Equal(OrderStatus.Cancelled, cancelled.Status);
    }

    [Fact]
    public async Task Cancel_SingleLine_ReturnsStock()
    {
        var book = await _app.AddBookAsync(stock: 5);
        var order = await _app.OrderService.PlaceOrderAsync(new PlaceOrderRequest(
            "reader@example.com",
            new List<OrderLineRequest> { new(book.Id, 2) }));

        await _app.OrderService.CancelAsync(order.Id);

        Assert.Equal(5, (await _app.Books.GetAsync(book.Id))!.StockQuantity);
    }

    [Fact]
    public async Task Cancel_MultiLine_ReturnsStockForEveryLine()
    {
        var bookA = await _app.AddBookAsync(stock: 5, title: "A");
        var bookB = await _app.AddBookAsync(stock: 5, title: "B");
        var order = await _app.OrderService.PlaceOrderAsync(new PlaceOrderRequest(
            "reader@example.com",
            new List<OrderLineRequest>
            {
                new(bookA.Id, 2),
                new(bookB.Id, 3),
            }));

        await _app.OrderService.CancelAsync(order.Id);

        Assert.Equal(5, (await _app.Books.GetAsync(bookA.Id))!.StockQuantity);
        Assert.Equal(5, (await _app.Books.GetAsync(bookB.Id))!.StockQuantity);
    }

    [Fact]
    public async Task Cancel_Twice_DoesNotRestockTwice()
    {
        var book = await _app.AddBookAsync(stock: 5);
        var order = await _app.OrderService.PlaceOrderAsync(new PlaceOrderRequest(
            "reader@example.com",
            new List<OrderLineRequest> { new(book.Id, 2) }));

        await _app.OrderService.CancelAsync(order.Id);
        await _app.OrderService.CancelAsync(order.Id);

        Assert.Equal(5, (await _app.Books.GetAsync(book.Id))!.StockQuantity);
    }
}
