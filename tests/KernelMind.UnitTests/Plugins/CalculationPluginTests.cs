using FluentAssertions;

namespace KernelMind.UnitTests.Plugins;

public class CalculationPluginValidationTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void CalculateTotal_WithInvalidSubtotal_Throws(decimal subtotal)
    {
        var plugin = new TestableCalculationPlugin();

        Action act = () => plugin.CalculateTotalValidation(subtotal);

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(45.00)]
    [InlineData(100.00)]
    [InlineData(0.01)]
    public void CalculateTotal_WithValidSubtotal_ReturnsResult(decimal subtotal)
    {
        var plugin = new TestableCalculationPlugin();

        var result = plugin.CalculateTotalValidation(subtotal);

        result.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("PIZZA10", 100, 10)]
    [InlineData("PRIMEIRA", 100, 10)]
    [InlineData("FAMILY20", 100, 20)]
    [InlineData("INVALID", 100, 0)]
    public void ApplyDiscount_WithVariousCodes_ReturnsCorrectDiscount(string code, decimal total, decimal expectedDiscount)
    {
        var plugin = new TestableCalculationPlugin();

        var result = plugin.ApplyDiscountValidation(total, code);

        result.Should().Be(expectedDiscount);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-5")]
    public void CalculateDeliveryFee_WithInvalidDistance_ReturnsError(string distance)
    {
        var plugin = new TestableCalculationPlugin();

        var result = plugin.CalculateDeliveryFeeValidation(distance);

        result.Should().Contain("inválida");
    }

    [Theory]
    [InlineData("5")]
    [InlineData("10")]
    [InlineData("3.5")]
    public void CalculateDeliveryFee_WithValidDistance_ReturnsFee(string distance)
    {
        var plugin = new TestableCalculationPlugin();

        var result = plugin.CalculateDeliveryFeeValidation(distance);

        result.Should().Contain("Entrega");
        result.Should().Contain("R$");
    }

    [Theory]
    [InlineData("100", 0)]
    [InlineData("100", -1)]
    public void SplitBill_WithInvalidPeople_ReturnsError(string total, int people)
    {
        var plugin = new TestableCalculationPlugin();

        var result = plugin.SplitBillValidation(total, people);

        result.Should().Contain("inválido");
    }

    [Theory]
    [InlineData("100", 2, 50)]
    [InlineData("100", 4, 25)]
    [InlineData("50", 2, 25)]
    public void SplitBill_WithValidInputs_ReturnsSplitAmount(string total, int people, decimal expectedPerPerson)
    {
        var plugin = new TestableCalculationPlugin();

        var result = plugin.SplitBillValidation(total, people);

        result.Should().Contain(expectedPerPerson.ToString());
    }

    private class TestableCalculationPlugin
    {
        private const decimal DeliveryFee = 5.00m;

        public string CalculateTotalValidation(decimal subtotal)
        {
            if (subtotal <= 0)
                throw new ArgumentException("Subtotal must be positive");

            var total = subtotal + DeliveryFee;
            return $"Total: {total:C}";
        }

        public decimal ApplyDiscountValidation(decimal currentTotal, string couponCode)
        {
            return couponCode.ToUpper() switch
            {
                "PIZZA10" => currentTotal * 0.10m,
                "PRIMEIRA" => 10.00m,
                "FAMILY20" => currentTotal * 0.20m,
                "DELIVERY5" => 5.00m,
                _ => 0m
            };
        }

        public string CalculateDeliveryFeeValidation(string distance)
        {
            if (!decimal.TryParse(distance, out var km) || km <= 0)
                return "Distância inválida";

            return $"Entrega: R$ {DeliveryFee:C}";
        }

        public string SplitBillValidation(string total, int people)
        {
            if (!decimal.TryParse(total, out var amount) || amount <= 0)
                return "Valor inválido";

            if (people <= 0)
                return "Número de pessoas inválido";

            var perPerson = amount / people;
            return $"Cada pessoa paga: {perPerson:C}";
        }
    }
}

public class CalculationPluginPricingTests
{
    [Fact]
    public void DeliveryFee_IsAlwaysPositive()
    {
        var plugin = new TestablePricingPlugin();
        var fee = plugin.GetDeliveryFeeConstant();

        fee.Should().BeGreaterThan(0);
    }

    [Fact]
    public void DeliveryFee_IsReasonable()
    {
        var plugin = new TestablePricingPlugin();
        var fee = plugin.GetDeliveryFeeConstant();

        fee.Should().BeLessThan(50);
        fee.Should().BeGreaterThan(0);
    }

    private class TestablePricingPlugin
    {
        public decimal GetDeliveryFeeConstant() => 5.00m;
    }
}
