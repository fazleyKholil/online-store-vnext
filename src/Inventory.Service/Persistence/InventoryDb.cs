using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Metrics;
using Infrastructure;
using Online.Store.Common.Dto;
using Serilog;

namespace Inventory.Service.Persistence
{
    public class InventoryDb : IInventoryDb
    {
        private readonly ILogger _logger;
        private readonly IMetrics _metrics;
        private readonly List<Product> _products;

        public InventoryDb(ILogger logger, IMetrics metrics)
        {
            _logger = logger;
            _metrics = metrics;
            _products = InitialiseProduct();
        }

        public async Task<List<Product>> GetAll()
        {
            return _products;
        }

        public async Task UpdateStock(string productId, int quantity)
        {
            _logger.Information("Updating Inventory");

            var product = _products.FirstOrDefault(p => p.ProductId == productId);

            if (product != null)
                product.Quantity -= quantity;
            else
                throw new Exception($"Product {productId} not found");

            //simulating db update
            await Task.Delay(ExternalServicesConst.InventoryLatency);

            _logger.Information("Updating Inventory completed");
        }

        private List<Product> InitialiseProduct()
        {
            _logger.Information("Initialising products");

            //fake db retrieve products
            return new List<Product>()
            {
                new Product {ProductId = "0acc81de-de63-45b5-864a-dd6637a75ca4", Description = "T-Shirt", Quantity = 100, BuyingPrice = 50, SellingPrice = 150},
                new Product {ProductId = "0727b58b-3681-4653-9f53-b9f4ec31e14f", Description = "Pant", Quantity = 50, BuyingPrice = 100, SellingPrice = 200},
                new Product {ProductId = "9dd785c6-5646-494a-b600-878180b16510", Description = "Watch", Quantity = 15, BuyingPrice = 50, SellingPrice = 100},
                new Product {ProductId = "b5e0263e-e856-4d91-940d-53408582bb91", Description = "Jacket", Quantity = 45, BuyingPrice = 200, SellingPrice = 250},
            };
        }
    }
}