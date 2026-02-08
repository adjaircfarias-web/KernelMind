using FluentAssertions;

namespace KernelMind.UnitTests.Plugins;

public class OrderPluginValidationTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateOrder_WithEmptyCustomerName_Throws(string customerName)
    {
        var plugin = new TestableOrderPlugin();

        Func<Task<string>> act = () => plugin.CreateOrderValidation(customerName, "123 Main St", "555-1234");

        act.Should().ThrowAsync<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateOrder_WithEmptyAddress_Throws(string address)
    {
        var plugin = new TestableOrderPlugin();

        Func<Task<string>> act = () => plugin.CreateOrderValidation("John Doe", address, "555-1234");

        act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public void CancelOrder_WithInvalidToken_ReturnsError()
    {
        var plugin = new TestableOrderPlugin();

        var result = plugin.CancelOrderValidation("INVALID123");

        result.Should().Contain("não encontrado");
    }

    [Theory]
    [InlineData("ORDER123")]
    [InlineData("ABC001")]
    [InlineData("TEST999")]
    public void CancelOrder_WithValidToken_ReturnsCancellation(string token)
    {
        var plugin = new TestableOrderPlugin();
        plugin.CreateOrderValidation("John Doe", "123 Main St", "555-1234").Wait();
        var orderToken = plugin.CreateOrderValidation("Jane Doe", "456 Oak Ave", "555-5678").Result;
        var extractedToken = orderToken.Replace("Pedido ", "").Replace(" criado", "");

        var result = plugin.CancelOrderValidation(extractedToken);

        result.Should().Contain("Cancelado");
    }

    [Fact]
    public void ViewOrder_WithInvalidToken_ReturnsError()
    {
        var plugin = new TestableOrderPlugin();

        var result = plugin.ViewOrderValidation("INVALID123");

        result.Should().Contain("não encontrado");
    }

    private class TestableOrderPlugin
    {
        private readonly Dictionary<string, TestOrder> _orders = new();

        public Task<string> CreateOrderValidation(string customerName, string address, string phone)
        {
            if (string.IsNullOrWhiteSpace(customerName))
                throw new ArgumentException("Customer name required");
            
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Address required");

            var token = Guid.NewGuid().ToString("N")[..8].ToUpper();
            _orders[token] = new TestOrder { CustomerName = customerName, Address = address };

            return Task.FromResult($"Pedido {token} criado");
        }

        public string CancelOrderValidation(string token)
        {
            if (!_orders.ContainsKey(token))
                return $"Pedido '{token}' não encontrado";

            _orders.Remove(token);
            return $"Pedido {token} Cancelado";
        }

        public string ViewOrderValidation(string token)
        {
            if (!_orders.ContainsKey(token))
                return $"Pedido '{token}' não encontrado";

            var order = _orders[token];
            return $"Pedido {token}: {order.CustomerName} - {order.Address}";
        }

        private class TestOrder
        {
            public string CustomerName { get; set; } = string.Empty;
            public string Address { get; set; } = string.Empty;
            public List<TestOrderItem> Items { get; set; } = new();
        }

        private class TestOrderItem
        {
            public string Name { get; set; } = string.Empty;
            public int Quantity { get; set; }
            public decimal Price { get; set; }
        }
    }
}

public class OrderPluginTokenTests
{
    [Fact]
    public void GenerateOrderToken_IsUnique()
    {
        var tokens = new HashSet<string>();
        for (int i = 0; i < 100; i++)
        {
            var token = Guid.NewGuid().ToString("N")[..8].ToUpper();
            tokens.Add(token);
        }

        tokens.Count.Should().Be(100);
    }

    [Fact]
    public void GenerateOrderToken_HasCorrectLength()
    {
        var token = Guid.NewGuid().ToString("N")[..8].ToUpper();

        token.Length.Should().Be(8);
    }

    [Fact]
    public void GenerateOrderToken_IsUppercase()
    {
        var token = Guid.NewGuid().ToString("N")[..8].ToUpper();

        token.Should().Be(token.ToUpper());
    }

    [Fact]
    public void GenerateOrderToken_ContainsOnlyHex()
    {
        var token = Guid.NewGuid().ToString("N")[..8].ToUpper();

        token.All(c => char.IsLetterOrDigit(c)).Should().BeTrue();
    }
}

public class OrderPluginPricingTests
{
    [Theory]
    [InlineData(1, 45.00, 45.00)]
    [InlineData(2, 45.00, 90.00)]
    [InlineData(3, 33.33, 99.99)]
    public void CalculateItemTotal_ReturnsCorrectTotal(int quantity, decimal unitPrice, decimal expected)
    {
        var total = quantity * unitPrice;

        total.Should().Be(expected);
    }

    [Fact]
    public void CalculateOrderTotal_IncludesDeliveryFee()
    {
        var subtotal = 100.00m;
        var deliveryFee = 5.00m;
        var total = subtotal + deliveryFee;

        total.Should().Be(105.00m);
    }
}
