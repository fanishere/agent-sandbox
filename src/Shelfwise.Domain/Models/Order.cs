namespace Shelfwise.Domain.Models;

public enum OrderStatus
{
    Pending,
    Paid,
    Shipped,
    Cancelled,
}

public sealed class OrderLine
{
    public required Guid BookId { get; init; }
    public required string Title { get; init; }
    public required int Quantity { get; init; }
    public required decimal UnitPrice { get; init; }

    /// <summary>Price for the whole line after any bulk discount.</summary>
    public decimal LineTotal { get; set; }
}

public sealed class Order
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string CustomerEmail { get; init; }
    public List<OrderLine> Lines { get; init; } = new();
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public DateTime CreatedAt { get; init; }
    public decimal Total { get; set; }
}
