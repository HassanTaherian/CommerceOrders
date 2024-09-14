﻿using Domain.ValueObjects;

namespace Contracts.Marketing.Send
{
    public class MarketingInvoiceRequest
    {
        public long InvoiceId { get; set; }
        public int UserId { get; set; }
        public InvoiceState InvoiceState { get; set; }
        public DateTime? ShopDateTime { get; set; }
    }
}
