using CommerceOrders.Api.Extensions;
using CommerceOrders.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<InvoiceDbContext, InvoiceDbContext>(options =>
    {
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("CommerceOrders")
        );
    }
);

builder.Services.AddPersistenceServices();
builder.Services.AddBusinessServices();
builder.Services.AddExternalServices();
// TODO: Inject HttpClient

var app = builder.Build();

// Configure the HTTP request pipeline.
app.ConfigureExceptionHandler(app.Environment);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();