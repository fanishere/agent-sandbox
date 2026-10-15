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
        Assert.Equal(90.00m, _pricing.PriceLine(10.00m, 10));
    }

    [Fact]
    public void PriceLine_WellAboveBulkThreshold_AppliesTenPercentDiscount()
    {
        Assert.Equal(99.00m, _pricing.PriceLine(10.00m, 11));
    }

    public static TheoryData<decimal, int, decimal> MidpointCases => new()
    {
        { 5.15m, 11, 50.99m },
        { 1.75m, 11, 17.33m },
        { 2.15m, 11, 21.29m },
        { 3.25m, 11, 32.18m },
        { 0.85m, 11, 8.42m },
    };

    [Theory]
    [MemberData(nameof(MidpointCases))]
    public void PriceLine_DiscountedMidpoint_RoundsAwayFromZero(decimal unitPrice, int quantity, decimal expected)
    {
        Assert.Equal(expected, _pricing.PriceLine(unitPrice, quantity));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void PriceLine_NonPositiveQuantity_Throws(int quantity)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _pricing.PriceLine(10.00m, quantity));
    }
}
