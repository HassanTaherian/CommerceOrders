namespace CommerceOrders.Contracts.UI.Cart
{
    public class AddProductRequestDto : BaseCartItem
    {
        public int UnitPrice { get; set; }

        public int Quantity { get; set; }
    }
}