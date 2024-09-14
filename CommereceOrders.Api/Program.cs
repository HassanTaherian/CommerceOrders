using CommerceOrders.Api.Extensions;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;
using CommerceOrders.Services.Abstractions;
using CommerceOrders.Services.External;
using CommerceOrders.Services.Mappers;
using CommerceOrders.Services.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<InvoiceContext, InvoiceContext>(options =>
    {
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("CommerceOrders")
        );
    }
);

// Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
// Services
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IReturningService, ReturningService>();
builder.Services.AddScoped<IDiscountService, DiscountService>();
builder.Services.AddScoped<ISecondCartService, SecondCartService>();
builder.Services.AddScoped<IRecommendService, RecommendService>();
builder.Services.AddSingleton<IHttpProvider, HttpProvider>();
// Adapters
builder.Services.AddScoped<IProductAdapter, ProductAdapter>();
builder.Services.AddScoped<IMarketingAdapter, MarketingAdapter>();
// Mapper
builder.Services.AddScoped<OrderMapper>();
// TODO: Inject HttpClient

var app = builder.Build();

// Configure the HTTP request pipeline.
app.ConfigureExceptionHandler(app.Environment);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();