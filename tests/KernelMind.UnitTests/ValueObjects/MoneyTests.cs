using FluentAssertions;
using KernelMind.Domain.ValueObjects;

namespace KernelMind.UnitTests.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void CreateMoney_WithValidAmount_ReturnsMoney()
    {
        var money = new Money(45.00m);

        money.Amount.Should().Be(45.00m);
    }

    [Fact]
    public void CreateMoney_WithZeroAmount_ReturnsZero()
    {
        var money = new Money(0);

        money.Amount.Should().Be(0);
    }

    [Fact]
    public void Equals_SameAmount_ReturnsTrue()
    {
        var money1 = new Money(45.00m);
        var money2 = new Money(45.00m);

        money1.Should().Be(money2);
    }

    [Fact]
    public void Equals_DifferentAmount_ReturnsFalse()
    {
        var money1 = new Money(45.00m);
        var money2 = new Money(50.00m);

        money1.Should().NotBe(money2);
    }

    [Fact]
    public void GetHashCode_SameAmount_ReturnsSameHash()
    {
        var money1 = new Money(45.00m);
        var money2 = new Money(45.00m);

        money1.GetHashCode().Should().Be(money2.GetHashCode());
    }

    [Fact]
    public void Money_Currency_ShouldBeBRL()
    {
        var money = new Money(100);

        money.Currency.Should().Be("BRL");
    }
}
