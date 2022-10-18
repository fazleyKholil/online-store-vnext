using System.Collections.Generic;
using System.Threading.Tasks;
using App.Metrics;
using Infrastructure.Instrumentation.Metrics;
using Infrastructure.Messaging.Aws.Sqs;
using Infrastructure.Sdk.Api;
using Online.Store.Common.Commands;
using Online.Store.Common.Dto;
using Refit;
using Serilog;

namespace Infrastructure.Sdk
{
    public class OnlineStoreSdk : IOnlineStoreSdk
    {
        private readonly ILogger _logger;
        private readonly IMetrics _metrics;
        private readonly IInventoryServiceApi _inventoryService;
        private readonly IShippingServiceApi _shippingService;
        private readonly MicroserviceApiOptions _options;
        private readonly IAwsSqs _awsSqs;

        public OnlineStoreSdk(ILogger logger
            , IMetrics metrics
            , MicroserviceApiOptions options,
            IAwsSqs awsSqs)
        {
            _logger = logger;
            _metrics = metrics;
            _options = options;
            _awsSqs = awsSqs;

            _inventoryService = RestService.For<IInventoryServiceApi>(_options.InventoryServiceUrl);
            _shippingService = RestService.For<IShippingServiceApi>(_options.ShippingServiceUrl);
        }

        public async Task<List<Product>> GetProducts()
        {
            using (_metrics.TimeOperation("get_products_inventory", "online_store_sdk"))
            {
                return await _inventoryService.Products();
            }
        }

        public async Task AdjustInventory(InventoryRequest request)
        {
            using (_metrics.TimeOperation("adjust_inventory", "online_store_sdk"))
            {
                await _inventoryService.Adjust(request);
            }
        }

        public async Task<ShippingResponse> ShippingInformation(ShippingRequest request)
        {
            using (_metrics.TimeOperation("get_info_shipping", "online_store_sdk"))
            {
                return await _shippingService.Info(request);
            }
        }

        public async Task PerformAccounting(AccountingCommand command)
        {
            using (_metrics.TimeOperation("perform_accounting_async", "online_store_sdk"))
            {
                await _awsSqs.SendMessageAsync(command);
            }
        }
    }
}