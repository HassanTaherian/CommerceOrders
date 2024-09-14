namespace Contracts.Discount
{
    public class DiscountResponseDto
    {
        public int TotalPrice { get; set; }

        public ICollection<DiscountProductResponseDto> Products { get; set; }
    }
}
