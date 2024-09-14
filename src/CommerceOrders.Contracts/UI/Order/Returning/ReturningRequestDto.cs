namespace CommerceOrders.Contracts.UI.Order.Returning
{
    public class ReturningRequestDto
    {
        public long InvoiceId { get; set; }
        public ICollection<int> ProductIds { get; set; }
    }
}