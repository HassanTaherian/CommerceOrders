using CommerceOrders.Domain.Entities;
using CommerceOrders.Domain.Exceptions;
using CommerceOrders.Domain.Exceptions.SecondCart;
using CommerceOrders.Domain.Repositories;
using CommerceOrders.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CommerceOrders.Persistence.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly InvoiceContext _dbContext;

        public InvoiceRepository(InvoiceContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Invoice?> GetInvoices() => _dbContext.Invoices;
        
        public IEnumerable<InvoiceItem> GetInvoiceItems() => _dbContext.InvoiceItems;

            public async Task<Invoice> GetInvoiceById(long id)
        {
            var invoice = await _dbContext.Invoices.Include(invoice => invoice.InvoiceItems)
                .SingleOrDefaultAsync(invoice => invoice.Id == id);

            if (invoice is null)
            {
                throw new InvoiceNotFoundException(id);
            }

            return invoice;
        }
        
        public async Task<Invoice> InsertInvoice(Invoice invoice)
        {
            await _dbContext.Invoices.AddAsync(invoice);
            return invoice;
        }

        public Invoice UpdateInvoice(Invoice invoice)
        {
            _dbContext.Invoices.Attach(invoice);
            _dbContext.Entry(invoice).State = EntityState.Modified;

            return invoice;
        }
        
        public IEnumerable<Invoice?> GetInvoiceByState(int userId, InvoiceState invoiceState)
        {
            var userInvoices = _dbContext.Invoices.Include(invoice => invoice.InvoiceItems)
                .Where(invoice => invoice.UserId == userId &&
                                  invoice.State == invoiceState).AsEnumerable();
            
            return userInvoices;
        }

        public Invoice GetCartOfUser(int userId)
        {
            var cart = GetInvoiceByState(userId, InvoiceState.CartState).FirstOrDefault();

            if (cart is null)
            {
                throw new CartNotFoundException(userId);
            }

            return cart;
        }
        
        public Invoice GetSecondCartOfUser(int userId)
        {
            var secondCart = GetInvoiceByState(userId, InvoiceState.SecondCartState).FirstOrDefault();

            if (secondCart is null)
            {
                throw new SecondCartNotFoundException(userId);
            }

            return secondCart;
        }

        public async Task<InvoiceItem> GetProductOfInvoice(long invoiceId, int productId)
        {
            var invoice = await GetInvoiceById(invoiceId);
            var invoiceItem = invoice.InvoiceItems.SingleOrDefault(invoiceItem => invoiceItem.ProductId == productId);

            if (invoiceItem is null)
            {
                throw new InvoiceItemNotFoundException(invoiceId, productId);
            }

            return invoiceItem;
        }
        
        public async Task<IEnumerable<InvoiceItem>> GetNotDeleteItems(long invoiceId)
        {
            var invoice = await GetInvoiceById(invoiceId);

            var invoiceItems = invoice.InvoiceItems.Where(item => item.IsDeleted == false);

            return invoiceItems;
        }
    }
}