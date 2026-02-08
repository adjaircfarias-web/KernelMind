using KernelMind.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace KernelMind.Core.Plugins;

/// <summary>
/// Plugin for price calculations
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
    /// Calculates the total price of an order including delivery fee
    /// </summary>
    public string CalculateTotal(decimal subtotal)
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
    /// Calculates order total with multiple items
    /// </summary>
    public string CalculateOrderTotal(string itemsJson)
    {
        _logger.LogInformation("Calculating order total from items");
        
        return $"📋 Para calcular o total do seu pedido, preciso saber os itens.\n\n" +
               $"🚚 **Taxa de entrega:** {DELIVERY_FEE:C}\n\n" +
               $"💡 Use *calculate_total* com o subtotal dos itens.";
    }

    /// <summary>
    /// Calculates delivery fee based on distance/zone
    /// </summary>
    public string CalculateDeliveryFee(string distance)
    {
        _logger.LogInformation("Calculating delivery fee for distance: {Distance}", distance);
        
        if (!decimal.TryParse(distance, out var km) || km <= 0)
        {
            return $"❌ Distância '{distance}' inválida. Informe a distância em quilômetros.";
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
    /// Estimates delivery time based on distance
    /// </summary>
    public string EstimateDeliveryTime(string distance)
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
    /// Checks today's promotion
    /// </summary>
    public string CheckPromotion()
    {
        var today = DateTime.UtcNow.DayOfWeek;
        var promotion = WeeklyPromotions[today];
        
        return $"🎁 **Promoção de {today}**\n\n" +
               $"📋 **{promotion.Name}**\n\n" +
               $"💡 Valida apenas para hoje! 🍕";
    }

    /// <summary>
    /// Applies a discount coupon to an order
    /// </summary>
    public string ApplyDiscount(decimal currentTotal, string couponCode)
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
    /// Gets the delivery fee
    /// </summary>
    public string GetDeliveryFee()
    {
        return $"🚚 **Taxa de Entrega:** {DELIVERY_FEE:C}\n\n" +
               $"📍 A taxa de entrega é fixa para todas as regiões até 3km.\n\n" +
               $"💡 Para regiões mais distantes, use *calculate_delivery_fee* com a distância.";
    }

    /// <summary>
    /// Splits the bill among people
    /// </summary>
    public string SplitBill(decimal total, int numberOfPeople)
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

    /// <summary>
    /// Calculates total with delivery included
    /// </summary>
    public string CalculateTotalWithDelivery(string subtotal, string distance)
    {
        _logger.LogInformation("Calculating total with delivery for: {Subtotal}, distance: {Distance}", 
            subtotal, distance);
        
        if (!decimal.TryParse(subtotal, out var sub) || sub <= 0)
            return $"❌ Subtotal '{subtotal}' inválido.";
        
        var deliveryFee = CalculateDeliveryFee(distance);
        
        var feeMatch = System.Text.RegularExpressions.Regex.Match(
            deliveryFee, @"Taxa: R\$ ([\d,]+)");
        var fee = feeMatch.Success ? decimal.Parse(feeMatch.Groups[1].Value.Replace(",", ".")) : DELIVERY_FEE;
        
        var total = sub + fee;
        
        return $"📊 **Total com Entrega**\n\n" +
               $"💰 **Subtotal:** {sub:C}\n" +
               $"🚚 **Entrega:** {fee:C}\n" +
               $"━━━━━━━━━━━━━━━━\n" +
               $"**TOTAL:** {total:C} 💵";
    }
}
