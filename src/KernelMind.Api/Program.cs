using KernelMind.Core;
using KernelMind.Core.Plugins;
using KernelMind.Domain.Interfaces;
using KernelMind.Infrastructure.Data;
using KernelMind.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPizzaRepository, PizzaRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IChatSessionRepository, ChatSessionRepository>();

builder.Services.AddScoped<MenuPlugin>();
builder.Services.AddScoped<OrderPlugin>();
builder.Services.AddScoped<CalculationPlugin>();
builder.Services.AddScoped<ContextPlugin>();

builder.Services.AddKernelMindServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
