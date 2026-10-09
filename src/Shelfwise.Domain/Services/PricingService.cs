namespace Shelfwise.Domain.Services;

/// <summary>
/// Pricing rules. Bulk rule: customers get 10% off a line when they buy
/// 10 or more copies of the same book.
/// </summary>
public sealed class PricingService
{
    public const int BulkThreshold = 10;
    public const decimal BulkDiscountRate = 0.10m;

    public decimal PriceLine(decimal unitPrice, int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive.");
        }

        var gross = unitPrice * quantity;

        if (quantity > BulkThreshold)
        {
            gross -= gross * BulkDiscountRate;
        }

        return Math.Round(gross, 2);
    }
}
