using Contracts.UI.SecondCart;
using Domain.Entities;

namespace Services.Abstractions
{
    public interface ISecondCartService
    {
        Invoice GetSecondCart(int userId);

        Task SecondCartToCart(ProductToSecondCartRequestDto productToSecondCartRequestDto);

        Task CartToSecondCart(ProductToSecondCartRequestDto productToSecondCartRequestDto);

        Task DeleteItemFromTheSecondCart(ProductToSecondCartRequestDto productToSecondCartRequestDto);
    }
}