namespace KernelMind.Domain.ValueObjects;

/// <summary>
/// Represents monetary value with currency
/// </summary>
public record Money
{
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "BRL";

    public Money(decimal amount, string currency = "BRL")
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money Zero(string currency = "BRL") => new(0, currency);
    
    public static Money operator +(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new InvalidOperationException("Cannot add money with different currencies");
        return new Money(a.Amount + b.Amount, a.Currency);
    }

    public static Money operator *(Money money, int quantity) => 
        new(money.Amount * quantity, money.Currency);

    public override string ToString() => $"{Currency} {Amount:N2}";
}
