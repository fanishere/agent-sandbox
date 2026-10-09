namespace Shelfwise.Domain.Models;

public sealed record CreateBookRequest(
    string Isbn,
    string Title,
    string Author,
    string? Genre,
    decimal Price,
    int StockQuantity);

public sealed record OrderLineRequest(Guid BookId, int Quantity);

public sealed record PlaceOrderRequest(string CustomerEmail, List<OrderLineRequest> Lines);
