using CommerceOrders.Contracts.UI.Cart;
using CommerceOrders.Services.Abstractions;
using CommerceOrders.Domain.Entities;
using CommerceOrders.Domain.Exceptions;
using CommerceOrders.Domain.Repositories;
using CommerceOrders.Domain.ValueObjects;

namespace CommerceOrders.Services.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInvoiceRepository _invoiceRepository;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _invoiceRepository = unitOfWork.InvoiceRepository;
        }

        public async Task AddCart(AddProductRequestDto addProductRequestDto, InvoiceState invoiceState)
        {
            var item = MapDtoToInvoiceItem(addProductRequestDto);

            try
            {
                var invoice = _invoiceRepository.GetCartOfUser(addProductRequestDto.UserId);
                await AddItemToInvoice(invoice, item);
            }
            catch (CartNotFoundException)
            {
                await AddItemToNewInvoice(addProductRequestDto.UserId, item);
            }
        }

        private static InvoiceItem MapDtoToInvoiceItem(AddProductRequestDto addProductRequestDto)
        {
            var item = new InvoiceItem
            {
                ProductId = addProductRequestDto.ProductId,
                Price = addProductRequestDto.UnitPrice,
                Quantity = addProductRequestDto.Quantity
            };
            return item;
        }

        private async Task AddItemToNewInvoice(int userId, InvoiceItem invoiceItem)
        {
            if (invoiceItem.Quantity <= 0)
            {
                throw new QuantityOutOfRangeInputException();
            }

            var newInvoice = new Invoice
            {
                UserId = userId,
                InvoiceItems = new List<InvoiceItem> { invoiceItem }
            };
            await _invoiceRepository.InsertInvoice(newInvoice);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task AddItemToInvoice(Invoice invoice, InvoiceItem invoiceItem)
        {
            if (invoiceItem.Quantity <= 0)
            {
                throw new QuantityOutOfRangeInputException();
            }

            try
            {
                var existedItem = await _invoiceRepository.GetProductOfInvoice(invoice.Id, invoiceItem.ProductId);
                existedItem.IsDeleted = false;
                existedItem.Quantity = invoiceItem.Quantity;
                existedItem.Price = invoiceItem.Price;
            }
            catch (InvoiceItemNotFoundException)
            {
                invoice.InvoiceItems.Add(invoiceItem);
            }

            _invoiceRepository.UpdateInvoice(invoice);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateQuantity(UpdateQuantityRequestDto updateQuantityRequestDto)
        {
            var cart = _invoiceRepository.GetCartOfUser(updateQuantityRequestDto.UserId);

            if (updateQuantityRequestDto.Quantity <= 0)
            {
                throw new QuantityOutOfRangeInputException();
            }

            var existed = await _invoiceRepository.GetProductOfInvoice(cart.Id, updateQuantityRequestDto.ProductId);

            existed.Quantity = updateQuantityRequestDto.Quantity;
            existed.IsDeleted = false;

            _invoiceRepository.UpdateInvoice(cart);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteItem(DeleteProductRequestDto deleteProductRequestDto)
        {
            var cart = _invoiceRepository.GetCartOfUser(deleteProductRequestDto.UserId);

            var existedItem = await _invoiceRepository.GetProductOfInvoice(cart.UserId, deleteProductRequestDto.ProductId);

            existedItem.IsDeleted = true;
            _invoiceRepository.UpdateInvoice(cart);
            await _unitOfWork.SaveChangesAsync();
        }

        public List<WatchInvoiceItemsResponseDto> ExistedCartItems(int userId)
        {
            var invoice = _invoiceRepository.GetCartOfUser(userId);
            var invoiceItems = GetNotDeletedItemsFromCart(invoice);
            if (!invoiceItems.Any())
            {
                throw new EmptyCartException(userId);
            }
            
            return invoiceItems;
        }

        private static List<WatchInvoiceItemsResponseDto> GetNotDeletedItemsFromCart(Invoice invoice)
        {
            return
                (
                from invoiceItem in invoice.InvoiceItems
                where !invoiceItem.IsDeleted
                select new WatchInvoiceItemsResponseDto
                {
                    ProductId = invoiceItem.ProductId,
                    Quantity = invoiceItem.Quantity,
                    UnitPrice = invoiceItem.Price
                }
                ).ToList();
        }

        public List<WatchInvoiceItemsResponseDto> IsDeletedCartItems(int userId)
        {
            var invoice = _invoiceRepository.GetCartOfUser(userId);
            var invoiceItems = GetDeletedItemsFromCart(invoice);

            if (!invoiceItems.Any())
            {
                throw new EmptyCartException(userId);
            }

            return invoiceItems;

        }

        private static List<WatchInvoiceItemsResponseDto> GetDeletedItemsFromCart(Invoice invoice)
        {
            return
            (
                from invoiceItem in invoice.InvoiceItems
                where invoiceItem.IsDeleted
                select new WatchInvoiceItemsResponseDto
                {
                    ProductId = invoiceItem.ProductId,
                    Quantity = invoiceItem.Quantity,
                    UnitPrice = invoiceItem.Price
                }
            ).ToList();
        }
    }
}