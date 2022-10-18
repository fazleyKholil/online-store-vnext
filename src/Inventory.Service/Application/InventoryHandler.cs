using System.Collections.Generic;
using System.Threading.Tasks;
using App.Metrics;
using Infrastructure;
using Infrastructure.Instrumentation.Metrics;
using Inventory.Service.Persistence;
using Online.Store.Common.Dto;
using Serilog;

namespace Inventory.Service.Application
{
    public class InventoryHandler : IInventoryHandler
    {
        private readonly ILogger _logger;
        private readonly IMetrics _metrics;
        private readonly IInventoryDb _inventoryDb;

        public InventoryHandler(ILogger logger
            , IMetrics metrics
            , IInventoryDb inventoryDb)
        {
            _logger = logger;
            _metrics = metrics;
            _inventoryDb = inventoryDb;
        }

        public Task<List<Product>> GetAllProducts()
        {
            _logger.Information("Retrieving products");

            using (_metrics.TimeOperation("get_all_products", "inventory_service"))
            {
                return _inventoryDb.GetAll();
            }
        }

        public async Task AdjustInventory(string productId, int quantity)
        {
            _logger.Information("Adjusting Inventory");

            using (_metrics.TimeOperation("update_stock", "inventory_service"))
            {
                await _inventoryDb.UpdateStock(productId, quantity);

                //other inventory related process like packaging box, goodies, etc tha will be bundled in the order but is optional
                await Task.Delay(ExternalServicesConst.InventoryOtherProcessLatency);
            }
        }
    }
}