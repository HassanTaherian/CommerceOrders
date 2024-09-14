using Contracts.Product;
using Contracts.UI.Recommendation;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using Domain.ValueObjects;
using Services.Abstractions;
using Services.External;

namespace Services.Services
{
    public class RecommendService : IRecommendService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IHttpProvider _httpProvider;

        public RecommendService(IInvoiceRepository invoiceRepository, IHttpProvider httpProvider)
        {
            _invoiceRepository = invoiceRepository;
            _httpProvider = httpProvider;
        }

        public async Task<List<ProductRecommendDto>> Recommended(RecommendationRequestDto recommendationRequestDto)
        {
            var items = await FindRelatedProducts(recommendationRequestDto);
            if (items.Count == 0)
            {
                throw new RelatedItemNotFoundException(recommendationRequestDto.ProductId);
            }

            return items;
        }

        private async Task<List<ProductRecommendDto>> FindRelatedProducts(
            RecommendationRequestDto recommendationRequestDto)
        {
            var relatedProductsFromModule = await GetRelatedProductFromProductModule(recommendationRequestDto);

            var productItemsFromDatabase = new List<ProductRecommendDto>();

            if (UserHasAnyInvoice(recommendationRequestDto.UserId))
            {
                productItemsFromDatabase = await GetDeletedInvoiceItemsOfUserFromDatabase(recommendationRequestDto);
            }

            var mostShoppedProducts = MostFrequentShoppedProducts();

            var concatItems = relatedProductsFromModule
                                                        .Concat(productItemsFromDatabase)
                                                        .Concat(mostShoppedProducts);

            return concatItems.DistinctBy(product => product.ProductId).ToList();
        }

        private async Task<List<ProductRecommendDto>> GetRelatedProductFromProductModule(RecommendationRequestDto recommendationRequestDto)
        {
            var productsResponse = await SerializeRecommendationRequestDto(recommendationRequestDto);

            return DeserializeRecommendationRequestDto(productsResponse);
        }

        private async Task<string> SerializeRecommendationRequestDto(RecommendationRequestDto recommendationRequestDto)
        {
            var mapItem = new ProductRecommendDto()
            {
                ProductId = recommendationRequestDto.ProductId
            };

            var jsonBridge = new JsonBridge<ProductRecommendDto, ProductRecommendDto>();
            var json = jsonBridge.Serialize(mapItem);
            var productResponse =
                await _httpProvider.Post("https://localhost:7083/mock/DiscountMock/Recommendation", json);
            return productResponse;
        }

        private List<ProductRecommendDto> DeserializeRecommendationRequestDto(string productResponseJson)
        {
            var jsonBridge = new JsonBridge<ProductRecommendDto, ProductRecommendDto>();
            var result = jsonBridge.DeserializeList(productResponseJson);

            if (result is null)
            {
                throw new Exception("Network not responding!");
            }

            return result;
        }

        private bool UserHasAnyInvoice(int userId)
        {
            return _invoiceRepository.GetInvoices()
                .Any(invoice => invoice != null &&
                invoice.UserId == userId &&
                invoice.State is InvoiceState.OrderState or InvoiceState.ReturnState
                );
        }


        private async Task<List<ProductRecommendDto>> GetDeletedInvoiceItemsOfUserFromDatabase(RecommendationRequestDto recommendationRequestDto)
        {
            var invoices = GetOrderAndReturnInvoicesOfUser(recommendationRequestDto.UserId);

            return await GetIsDeletedProductItemsOfUser(invoices, recommendationRequestDto.ProductId);
        }

        private IEnumerable<Invoice?> GetOrderAndReturnInvoicesOfUser(int userId)
        {
            var orderInvoices =
                _invoiceRepository.GetInvoiceByState(userId, InvoiceState.OrderState);
            var returnInvoices =
                _invoiceRepository.GetInvoiceByState(userId, InvoiceState.ReturnState);

            var invoices = orderInvoices.Concat(returnInvoices).ToList();

            return invoices;
        }

        private static Task<List<ProductRecommendDto>> GetIsDeletedProductItemsOfUser(IEnumerable<Invoice?> invoices, int productId)
        {
            return Task.FromResult((from invoice in invoices
                                    from invoiceItem in invoice.InvoiceItems
                                    where invoiceItem.IsDeleted && invoiceItem.ProductId != productId
                                    select invoiceItem)
                .Select(invoiceItem => new ProductRecommendDto
                { ProductId = invoiceItem.ProductId }).ToList());
        }

        private IEnumerable<ProductRecommendDto> MostFrequentShoppedProducts()
        {

            var products = GetMostFrequentShoppedProductsFromDatabase();


            return MapIntToProductRecommendDto(products);
        }

        private IEnumerable<int> GetMostFrequentShoppedProductsFromDatabase()
        {
            return 
            (
                from item in _invoiceRepository.GetInvoiceItems()
                where !item.IsDeleted && !item.IsReturn
                group item by item.ProductId
                into productGroup
                orderby productGroup.Count() descending
                select productGroup.Key
            ).Take(5);
        }

        private IEnumerable<ProductRecommendDto> MapIntToProductRecommendDto(IEnumerable<int> Items)
        {
            return Items.Select(id => new ProductRecommendDto
            {
                ProductId = id
            });
        }

    }
}