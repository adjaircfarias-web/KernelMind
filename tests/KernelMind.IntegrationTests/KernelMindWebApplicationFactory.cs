using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using KernelMind.Domain.Entities;
using KernelMind.Infrastructure.Data;
using KernelMind.Api.DTOs;

namespace KernelMind.IntegrationTests;

public class KernelMindWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDatabase");
            });

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();

            SeedTestData(db);
        });

        builder.UseEnvironment("Testing");
    }

    private void SeedTestData(AppDbContext db)
    {
        if (!db.Pizzas.Any())
        {
            db.Pizzas.AddRange(
                new Pizza
                {
                    Id = Guid.NewGuid(),
                    Name = "Margherita",
                    Description = "Classic Italian pizza with tomato sauce, mozzarella, and basil",
                    Price = 45.00m,
                    Category = "Tradicionais",
                    Ingredients = new List<string> { "Tomato sauce", "Mozzarella", "Basil" },
                    CreatedAt = DateTime.UtcNow
                },
                new Pizza
                {
                    Id = Guid.NewGuid(),
                    Name = "Pepperoni",
                    Description = "Popular American pizza with pepperoni slices",
                    Price = 52.00m,
                    Category = "Tradicionais",
                    Ingredients = new List<string> { "Tomato sauce", "Mozzarella", "Pepperoni" },
                    CreatedAt = DateTime.UtcNow
                },
                new Pizza
                {
                    Id = Guid.NewGuid(),
                    Name = "Quatro Queijos",
                    Description = "Four cheese pizza with mozzarella, provolone, gorgonzola, and parmesan",
                    Price = 58.00m,
                    Category = "Especiais",
                    Ingredients = new List<string> { "Mozzarella", "Provolone", "Gorgonzola", "Parmesan" },
                    CreatedAt = DateTime.UtcNow
                }
            );
            db.SaveChanges();
        }
    }
}
