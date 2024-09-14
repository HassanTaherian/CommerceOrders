namespace Contracts.Discount
{
    public class DiscountProductRequestDto
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public double UnitPrice { get; set; }
    }
}