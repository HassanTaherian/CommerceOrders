namespace Contracts.UI.Discount
{
    public class DiscountCodeRequestDto
    {
        public int UserId { get; set; }

        public string? DiscountCode { get; set; }
    }
}