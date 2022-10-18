using System;
using System.Threading.Tasks;
using App.Metrics;
using Infrastructure.Instrumentation.Metrics;
using Microsoft.AspNetCore.Mvc;
using Online.Store.Common.Dto;
using Serilog;

namespace Inventory.Service.Application
{
    [ApiController]
    [Route("[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMetrics _metrics;
        private readonly IInventoryHandler _handler;

        public InventoryController(ILogger logger
            , IMetrics metrics
            , IInventoryHandler handler)
        {
            _logger = logger;
            _metrics = metrics;
            _handler = handler;
        }

        [HttpGet]
        [Route("products")]
        public async Task<IActionResult> Products()
        {
            _metrics.IncrementOperation("products_inventory_count", "inventory_service");

            try
            {
                _logger.Information("Start processing inventory get products request");

                using (_metrics.TimeOperation("products_inventory", "inventory_service"))
                {
                    var result = await _handler.GetAllProducts();

                    _metrics.IncrementOperation("products_inventory_success_count", "inventory_service");

                    _logger.Information("Processing inventory products request completed");

                    return Ok(result);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "An error occured while processing inventory products");
                _metrics.IncrementOperation("products_inventory_error_count", "inventory_service");
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("adjust")]
        public async Task<IActionResult> Adjust(InventoryRequest request)
        {
            _metrics.IncrementOperation("adjust_inventory_count", "inventory_service");

            try
            {
                _logger.Information("Start processing inventory adjustment request");

                using (_metrics.TimeOperation("adjust_inventory", "inventory_service"))
                {
                    await _handler.AdjustInventory(request.ProductId, request.Quantity);

                    _metrics.IncrementOperation("adjust_inventory_success_count", "inventory_service");

                    _logger.Information("Processing inventory adjustment request completed");

                    return Ok();
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "An error occured while processing inventory adjustment");
                _metrics.IncrementOperation("adjust_inventory_error_count", "inventory_service");
                return BadRequest();
            }
        }
    }
}