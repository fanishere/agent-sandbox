using Shelfwise.Domain.Services;

namespace Shelfwise.Tests;

public class PricingServiceTests
{
    private readonly PricingService _pricing = new();

    [Fact]
    public void PriceLine_SingleCopy_ReturnsUnitPrice()
    {
        Assert.Equal(12.50m, _pricing.PriceLine(12.50m, 1));
    }

    [Fact]
    public void PriceLine_BelowBulkThreshold_AppliesNoDiscount()
    {
        Assert.Equal(90.00m, _pricing.PriceLine(10.00m, 9));
    }

    [Fact]
    public void PriceLine_ExactlyAtBulkThreshold_AppliesTenPercentDiscount()
    {
        // "10% off when you buy 10 or more copies": quantity 10 must be discounted.
        Assert.Equal(90.00m, _pricing.PriceLine(10.00m, 10));
    }

    [Fact]
    public void PriceLine_WellAboveBulkThreshold_AppliesTenPercentDiscount()
    {
        Assert.Equal(99.00m, _pricing.PriceLine(10.00m, 11));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void PriceLine_NonPositiveQuantity_Throws(int quantity)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _pricing.PriceLine(10.00m, quantity));
    }

    [Fact]
    public void PriceLine_BulkDiscountMidpoint_RoundsHalfUp()
    {
        // Finance example: 11 x 5.15 => gross 56.65, minus 10% = 50.985 => expect 50.99.
        Assert.Equal(50.99m, _pricing.PriceLine(5.15m, 11));
    }

    [Theory]
    [InlineData(5.15, 11, 50.99)]   // 50.985 -> away from zero
    [InlineData(0.75, 11, 7.43)]    // 7.425 -> away from zero
    [InlineData(1.05, 15, 14.18)]   // 14.175 -> away from zero
    [InlineData(0.85, 10, 7.65)]    // 7.65 exact, no rounding needed
    [InlineData(0.55, 10, 4.95)]    // 4.95 exact
    [InlineData(1.05, 10, 9.45)]    // 9.45 exact
    [InlineData(0.75, 12, 8.10)]    // 9.00 - 0.90 = 8.10 exact
    [InlineData(3.25, 10, 29.25)]   // 32.50 - 3.25 = 29.25 exact
    [InlineData(1.15, 10, 10.35)]   // 11.50 - 1.15 = 10.35 exact
    public void PriceLine_BulkDiscount_RoundsHalfUpAtTwoDecimals(
        decimal unitPrice, int quantity, decimal expected)
    {
        Assert.Equal(expected, _pricing.PriceLine(unitPrice, quantity));
    }
}
