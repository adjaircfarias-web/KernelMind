using Microsoft.Extensions.Logging;

namespace KernelMind.Core.Plugins;

/// <summary>
/// Plugin for price calculations
/// </summary>
public class CalculationPlugin
{
    private readonly ILogger<CalculationPlugin> _logger;
    private const decimal DELIVERY_FEE = 5.00m;

    public CalculationPlugin(ILogger<CalculationPlugin> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Calculates the total price of an order including delivery fee
    /// </summary>
    public string CalculateTotal(decimal subtotal)
    {
        _logger.LogInformation("Calculating total for subtotal: {Subtotal}", subtotal);
        
        var total = subtotal + DELIVERY_FEE;
        
        return $"Resumo do pedido:\n" +
               $"Subtotal: {subtotal:C}\n" +
               $"Taxa de entrega: {DELIVERY_FEE:C}\n" +
               $"**Total: {total:C}**";
    }

    /// <summary>
    /// Calculates order total with multiple items
    /// </summary>
    public string CalculateOrderTotal(string itemsJson)
    {
        _logger.LogInformation("Calculating order total from items");
        
        return $"Para calcular o total do seu pedido, preciso saber os itens.\n\n" +
               $"Taxa de entrega: {DELIVERY_FEE:C}\n" +
               $"Use calculate_total com o subtotal dos itens.";
    }

    /// <summary>
    /// Applies a discount coupon to an order
    /// </summary>
    public string ApplyDiscount(decimal currentTotal, string couponCode)
    {
        _logger.LogInformation("Applying discount code: {CouponCode}", couponCode);
        
        // Simple discount logic
        var discount = couponCode.ToUpper() switch
        {
            "PIZZA10" => currentTotal * 0.10m,
            "PRIMEIRA" => 10.00m,
            _ => 0m
        };

        if (discount == 0)
            return $"Código '{couponCode}' inválido ou expirado.";

        var newTotal = currentTotal - discount;
        return $"Desconto aplicado!\n" +
               $"Código: {couponCode}\n" +
               $"Desconto: {discount:C}\n" +
               $"**Novo total: {newTotal:C}**";
    }
}
