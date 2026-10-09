namespace Shelfwise.Domain.Models;

public sealed class Book
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Isbn { get; set; }
    public required string Title { get; set; }
    public required string Author { get; set; }
    public string? Genre { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public DateTime AddedAt { get; set; }
}
