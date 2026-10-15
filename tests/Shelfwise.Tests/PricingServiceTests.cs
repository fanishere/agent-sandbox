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

    [Theory]
    [InlineData(5.15, 11, 50.99)]   // 56.65 - 10% = 50.985 -> 50.99
    [InlineData(10.05, 11, 99.50)]  // 110.55 - 10% = 99.495 -> 99.50
    [InlineData(2.25, 11, 22.28)]   // 24.75 - 10% = 22.275 -> 22.28
    [InlineData(7.55, 11, 74.75)]   // 83.05 - 10% = 74.745 -> 74.75
    [InlineData(3.35, 11, 33.17)]   // 36.85 - 10% = 33.165 -> 33.17
    public void PriceLine_DiscountedMidpoint_RoundsAwayFromZero(double unitPrice, int quantity, double expected)
    {
        Assert.Equal((decimal)expected, _pricing.PriceLine((decimal)unitPrice, quantity));
    }
}
