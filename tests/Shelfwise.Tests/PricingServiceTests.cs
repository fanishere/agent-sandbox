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

    [Theory]
    [InlineData(10, "90.00")]
    [InlineData(11, "99.00")]
    public void PriceLine_AtOrAboveBulkThreshold_AppliesTenPercentDiscount(int quantity, string expected)
    {
        Assert.Equal(decimal.Parse(expected), _pricing.PriceLine(10.00m, quantity));
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
}
