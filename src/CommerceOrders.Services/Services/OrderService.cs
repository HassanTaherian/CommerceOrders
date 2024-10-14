﻿using CommerceOrders.Contracts.UI.Invoice;
using CommerceOrders.Contracts.UI.Order.Checkout;
using CommerceOrders.Domain.Exceptions.Order;
using Microsoft.EntityFrameworkCore;

namespace CommerceOrders.Services.Services;

internal class OrderService : IOrderService
{
    private readonly IUnitOfWork _uow;
    private readonly InvoiceService _invoiceService;
    private readonly IProductAdapter _productAdapter;
    private readonly IMarketingAdapter _marketingAdapter;
    private readonly ICartService _cartService;

    public OrderService(IUnitOfWork uow, InvoiceService invoiceService, IMarketingAdapter marketingAdapter,
        IProductAdapter productAdapter, ICartService cartService)
    {
        _marketingAdapter = marketingAdapter;
        _productAdapter = productAdapter;
        _cartService = cartService;
        _invoiceService = invoiceService;
        _uow = uow;
    }

    public async Task Checkout(CheckoutCommandRequest request)
    {
        Invoice? cart = await _cartService.GetCartWithItems(request.UserId);
        
        ValidateCart(cart, request);
        
        await _productAdapter.UpdateCountingOfProduct(cart!.InvoiceItems, ProductCountingState.ShopState);
        
        cart.State = InvoiceState.Order;
        cart.CreatedAt = DateTime.Now;
        await _uow.SaveChangesAsync();
        
        await _marketingAdapter.SendInvoiceToMarketing(cart, InvoiceState.Order);
    }

    private static void ValidateCart(Invoice? cart, CheckoutCommandRequest request)
    {
        if (cart is null)
        {
            throw new CartNotFoundException(request.UserId);
        }
        
        if (cart.AddressId is null)
        {
            throw new AddressNotSpecifiedException(cart.UserId);
        }

        if (cart.InvoiceItems.Count == 0)
        {
            throw new EmptyCartException(cart.UserId);
        }
    }
    
    public async Task<IEnumerable<OrderQueryResponse>> GetOrders(int userId)
    {
        List<OrderQueryResponse> orders = await _invoiceService.GetInvoices(userId, InvoiceState.Order)
            .AsNoTracking()
            .ToOrderQueryResponse()
            .ToListAsync();

        if (orders.Count == 0)
        {
            return Enumerable.Empty<OrderQueryResponse>();
        }

        return orders;
    }

    public async Task<OrderWithItemsQueryResponse> GetOrderWithItems(long orderId)
    {
        OrderWithItemsQueryResponse? order = await _uow.Set<Invoice>()
            .AsNoTracking()
            .Where(o => o.Id == orderId)
            .Include(invoice => invoice.InvoiceItems.Where(item => item.IsDeleted == false))
            .ToOrderWithItemsQueryResponse()
            .FirstOrDefaultAsync();

        if (order is null)
        {
            throw new OrderNotFoundException(orderId);
        }

        return order;
    }
}