using KernelMind.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace KernelMind.Core.Plugins;

/// <summary>
/// Plugin for price calculations with Semantic Kernel
/// </summary>
public class CalculationPlugin
{
    private readonly ILogger<CalculationPlugin> _logger;
    private readonly IPizzaRepository _pizzaRepository;
    private const decimal DELIVERY_FEE = 5.00m;
    
    private static readonly Dictionary<string, decimal> ZoneDeliveryFees = new()
    {
        ["centro"] = 5.00m,
        ["zona norte"] = 7.00m,
        ["zona sul"] = 7.00m,
        ["zona leste"] = 8.00m,
        ["zona oeste"] = 8.00m,
        ["outros"] = 10.00m
    };

    private static readonly Dictionary<DayOfWeek, (decimal Discount, string Name)> WeeklyPromotions = new()
    {
        [DayOfWeek.Monday] = (0.05m, "5% de desconto na primeira pizza"),
        [DayOfWeek.Tuesday] = (0.10m, "10% OFF em pizzas tradicionais"),
        [DayOfWeek.Wednesday] = (0m, "Promoção 2 pizzas + 1 grátis (2a pizza menor)"),
        [DayOfWeek.Thursday] = (0.08m, "8% OFF para clientes fidelidade"),
        [DayOfWeek.Friday] = (0m, "Promoção especial de sexta!"),
        [DayOfWeek.Saturday] = (0m, "Sábado de pizza!"),
        [DayOfWeek.Sunday] = (0.10m, "10% OFF no Família")
    };

    public CalculationPlugin(
        ILogger<CalculationPlugin> logger,
        IPizzaRepository pizzaRepository)
    {
        _logger = logger;
        _pizzaRepository = pizzaRepository;
    }

    /// <summary>
    /// Calculates the total price of an order including delivery fee.
    /// Use this to show the customer the final price.
    /// </summary>
    [KernelFunction("calculate_total")]
    [Description("Calculates the total price of an order including delivery fee")]
    public string CalculateTotal(
        [Description("The subtotal amount (sum of all items)")]
        decimal subtotal)
    {
        _logger.LogInformation("Calculating total for subtotal: {Subtotal}", subtotal);
        
        var total = subtotal + DELIVERY_FEE;
        
        return $"📊 **Resumo do Pedido**\n\n" +
               $"💰 **Subtotal:** {subtotal:C}\n" +
               $"🚚 **Taxa de entrega:** {DELIVERY_FEE:C}\n" +
               $"━━━━━━━━━━━━━━━━\n" +
               $"**TOTAL:** {total:C} 💵";
    }

    /// <summary>
    /// Calculates delivery fee based on distance or zone.
    /// Use this when the customer asks about delivery fees for their location.
    /// </summary>
    [KernelFunction("calculate_delivery_fee")]
    [Description("Calculates delivery fee based on distance or zone name")]
    public string CalculateDeliveryFee(
        [Description("Distance in kilometers (e.g., '5') or zone name (e.g., 'centro', 'zona norte')")]
        string distance)
    {
        _logger.LogInformation("Calculating delivery fee for distance: {Distance}", distance);
        
        // Check if it's a zone name
        if (ZoneDeliveryFees.TryGetValue(distance.ToLower(), out var zoneFee))
        {
            return $"🚚 **Taxa de Entrega**\n\n" +
                   $"🏷️ **Zona:** {distance}\n" +
                   $"💰 **Taxa:** {zoneFee:C}\n\n" +
                   $"💡 Para entregas até 3km, a taxa é {DELIVERY_FEE:C}.";
        }
        
        // Try to parse as distance
        if (!decimal.TryParse(distance, out var km) || km <= 0)
        {
            return $"❌ Distância '{distance}' inválida. Informe a distância em quilômetros ou o nome da zona.";
        }
        
        var zone = km switch
        {
            <= 3 => "centro",
            <= 6 => "zona norte",
            <= 8 => "zona sul",
            <= 12 => "zona leste",
            <= 15 => "zona oeste",
            _ => "outros"
        };
        
        var fee = ZoneDeliveryFees[zone];
        
        return $"🚚 **Taxa de Entrega**\n\n" +
               $"📍 **Distância:** {km:F1} km\n" +
               $"🏷️ **Zona:** {zone}\n" +
               $"💰 **Taxa:** {fee:C}\n\n" +
               $"💡 Para distâncias maiores que 15km, consulte-nos.";
    }

    /// <summary>
    /// Estimates delivery time based on distance.
    /// Use this when the customer asks how long delivery will take.
    /// </summary>
    [KernelFunction("estimate_delivery_time")]
    [Description("Estimates delivery time in minutes based on distance")]
    public string EstimateDeliveryTime(
        [Description("Distance in kilometers")]
        string distance)
    {
        _logger.LogInformation("Estimating delivery time for distance: {Distance}", distance);
        
        if (!decimal.TryParse(distance, out var km) || km <= 0)
        {
            return $"❌ Distância '{distance}' inválida.";
        }
        
        var baseTime = km <= 3 ? 25 : 30 + (int)(km * 2);
        var maxTime = baseTime + 15;
        
        return $"⏱️ **Tempo Estimado de Entrega**\n\n" +
               $"📍 **Distância:** {km:F1} km\n" +
               $"⏰ **Tempo:** {baseTime}-{maxTime} minutos\n\n" +
               $"💡 O tempo pode variar conforme o trânsito e a demanda.";
    }

    /// <summary>
    /// Checks today's promotion.
    /// Use this when the customer asks about current promotions or discounts.
    /// </summary>
    [KernelFunction("check_promotion")]
    [Description("Checks today's special promotion or discount")]
    public string CheckPromotion()
    {
        var today = DateTime.UtcNow.DayOfWeek;
        var promotion = WeeklyPromotions[today];
        
        return $"🎁 **Promoção de {today}**\n\n" +
               $"📋 **{promotion.Name}**\n\n" +
               $"💡 Valida apenas para hoje! 🍕";
    }

    /// <summary>
    /// Applies a discount coupon to an order.
    /// Use this when the customer has a coupon code.
    /// </summary>
    [KernelFunction("apply_discount")]
    [Description("Applies a discount coupon code to calculate the new total")]
    public string ApplyDiscount(
        [Description("Current total amount")]
        decimal currentTotal, 
        [Description("Coupon code (e.g., 'PIZZA10', 'PRIMEIRA')")]
        string couponCode)
    {
        _logger.LogInformation("Applying discount code: {CouponCode}", couponCode);
        
        var discount = couponCode.ToUpper() switch
        {
            "PIZZA10" => currentTotal * 0.10m,
            "PRIMEIRA" => 10.00m,
            "FAMILY20" => currentTotal * 0.20m,
            "DELIVERY5" => 5.00m,
            _ => 0m
        };

        if (discount == 0)
            return $"❌ Código '{couponCode}' inválido ou expirado.";

        var newTotal = currentTotal - discount;
        return $"🎉 **Desconto Aplicado!**\n\n" +
               $"📋 **Código:** {couponCode}\n" +
               $"💰 **Desconto:** {discount:C}\n" +
               $"━━━━━━━━━━━━━━━━\n" +
               $"**Novo total:** {newTotal:C} 💵";
    }

    /// <summary>
    /// Gets the standard delivery fee.
    /// Use this when the customer asks about delivery fees in general.
    /// </summary>
    [KernelFunction("get_delivery_fee")]
    [Description("Gets the standard delivery fee")]
    public string GetDeliveryFee()
    {
        return $"🚚 **Taxa de Entrega:** {DELIVERY_FEE:C}\n\n" +
               $"📍 A taxa de entrega é fixa para todas as regiões até 3km.\n\n" +
               $"💡 Para regiões mais distantes, use *calculate_delivery_fee* com a distância.";
    }

    /// <summary>
    /// Splits the bill among multiple people.
    /// Use this when the customer wants to know how much each person should pay.
    /// </summary>
    [KernelFunction("split_bill")]
    [Description("Splits the bill amount among multiple people")]
    public string SplitBill(
        [Description("Total bill amount")]
        decimal total, 
        [Description("Number of people to split the bill")]
        int numberOfPeople)
    {
        _logger.LogInformation("Splitting bill: {Total} among {People} people", 
            total, numberOfPeople);
        
        if (numberOfPeople <= 0)
            return $"❌ Número de pessoas inválido. Deve ser maior que 0.";
        
        if (numberOfPeople > 20)
            return $"❌ Não é possível dividir para mais de 20 pessoas.";
        
        var splitAmount = total / numberOfPeople;
        
        return $"💰 **Divisão da Conta**\n\n" +
               $"📋 **Total:** {total:C}\n" +
               $"👥 **Pessoas:** {numberOfPeople}\n" +
               $"━━━━━━━━━━━━━━━━\n" +
               $"**Cada pessoa paga:** {splitAmount:C}\n\n" +
               $"💡 Taxa de serviço já incluída!";
    }
}
