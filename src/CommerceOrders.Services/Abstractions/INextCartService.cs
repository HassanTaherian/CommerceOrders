using CommerceOrders.Contracts.UI.NextCart;

namespace CommerceOrders.Services.Abstractions;

public interface INextCartService
{
    Task<NextCartResponseDto> GetNextCart(int userId);

    Task MoveNextCartItemToCart(MoveBetweenNextCartAndCartDto moveBetweenNextCartAndCartDto);

    Task MoveCartItemToNextCart(MoveBetweenNextCartAndCartDto moveBetweenNextCartAndCartDto);

    Task DeleteNextCartItem(MoveBetweenNextCartAndCartDto moveBetweenNextCartAndCartDto);
}