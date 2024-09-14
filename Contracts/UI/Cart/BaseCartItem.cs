namespace Contracts.UI.Cart
{
    public abstract class BaseCartItem
    {
        public int UserId { get; set; }

        public int ProductId { get; set; }
    }
}