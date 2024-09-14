using CommerceOrders.Contracts.UI.SecondCart;
using CommerceOrders.Domain.Entities;

namespace CommerceOrders.Services.Abstractions;

public interface ISecondCartService
{
    Invoice GetSecondCart(int userId);

    Task SecondCartToCart(ProductToSecondCartRequestDto productToSecondCartRequestDto);

    Task CartToSecondCart(ProductToSecondCartRequestDto productToSecondCartRequestDto);

    Task DeleteItemFromTheSecondCart(ProductToSecondCartRequestDto productToSecondCartRequestDto);
}