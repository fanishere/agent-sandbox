namespace Shelfwise.Domain.Errors;

/// <summary>A business rule was violated. Maps to HTTP 400.</summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}

/// <summary>A requested entity does not exist. Maps to HTTP 404.</summary>
public sealed class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}

/// <summary>Not enough copies on the shelf. Maps to HTTP 409.</summary>
public sealed class InsufficientStockException : Exception
{
    public Guid BookId { get; }
    public int Requested { get; }
    public int Available { get; }

    public InsufficientStockException(Guid bookId, int requested, int available)
        : base($"Book {bookId}: requested {requested} but only {available} in stock.")
    {
        BookId = bookId;
        Requested = requested;
        Available = available;
    }
}
