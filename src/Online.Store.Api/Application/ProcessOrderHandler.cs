using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Metrics;
using Infrastructure.Sdk;
using Online.Store.Common.Commands;
using Online.Store.Common.Dto;
using Serilog;

namespace Online.Store.Api.Application
{
    public class ProcessOrderHandler : IProcessOrderHandler
    {
        private readonly ILogger _logger;
        private readonly IMetrics _metrics;
        private readonly IOnlineStoreSdk _sdk;

        public ProcessOrderHandler(ILogger logger
            , IMetrics metrics
            , IOnlineStoreSdk sdk)
        {
            _logger = logger;
            _metrics = metrics;
            _sdk = sdk;
        }

        public async Task<SaleResponse> Handle(OrderRequest request)
        {
            //process order
            var sale = new Sale();

            //get products from microservice
            var products = await _sdk.GetProducts();

            //set related product in sale 
            var total = 0M;
            foreach (var product in request.Products)
            {
                var relatedProduct = products.First(p => p.ProductId == product.ProductId);
                relatedProduct.Quantity = product.Quantity;
                sale.Products.Add(relatedProduct);
                total += relatedProduct.SellingPrice * product.Quantity;
            }

            //get shipping details from microservice
            var shippingResult = await _sdk.ShippingInformation(new ShippingRequest
            {
                Distance = request.ShippingDistance,
                IsExpress = request.IsShippingExpress
            });

            sale.ShippingCost = shippingResult.Shipment.Cost;

            //update inventory from microservice
            foreach (var product in sale.Products)
            {
                await _sdk.AdjustInventory(new InventoryRequest
                {
                    ProductId = product.ProductId,
                    Quantity = product.Quantity
                });
            }

            //perform accounting from microservice
            await _sdk.PerformAccounting(new AccountingCommand
            {
                Sale = sale
            });

            //build response
            var response = new SaleResponse
            {
                Total = total + sale.ShippingCost,
                ShippingInformation = shippingResult.Shipment,
                Products = new List<object> {sale.Products.Select(p => new {p.Description, p.Quantity})}
            };

            return response;
        }
    }
}