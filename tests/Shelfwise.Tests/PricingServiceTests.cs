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
    public void PriceLine_FinanceReconciliationExample_RoundsUpToOneCent()
    {
        // 11 x 5.15 = 56.65, minus 10% = 50.985 -> finance expects 50.99.
        Assert.Equal(50.99m, _pricing.PriceLine(5.15m, 11));
    }

    [Theory]
    [InlineData(5.15, 11, 50.99)]   // 50.985
    [InlineData(10.05, 11, 99.50)]  // 99.495
    [InlineData(1.05, 11, 10.40)]   // 10.395
    [InlineData(0.55, 11, 5.45)]    // 5.445
    [InlineData(12.55, 21, 237.20)] // 237.195
    public void PriceLine_Midpoints_RoundAwayFromZero(decimal unitPrice, int quantity, decimal expected)
    {
        Assert.Equal(expected, _pricing.PriceLine(unitPrice, quantity));
    }
}
