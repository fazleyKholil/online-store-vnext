using System;
using System.Threading.Tasks;
using App.Metrics;
using Infrastructure.Instrumentation.Metrics;
using Microsoft.AspNetCore.Mvc;
using Online.Store.Common.Dto;
using Serilog;

namespace Shipping.Service.Application
{
    [ApiController]
    [Route("[controller]")]
    public class ShippingController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMetrics _metrics;
        private readonly IShippingHandler _shippingHandler;

        public ShippingController(ILogger logger
            , IMetrics metrics
            , IShippingHandler shippingHandler)
        {
            _logger = logger;
            _metrics = metrics;
            _shippingHandler = shippingHandler;
        }

        [HttpPost]
        [Route("info")]
        public async Task<IActionResult> Info(ShippingRequest request)
        {
            _metrics.IncrementOperation("shipping_info_count", "shipping_service");

            try
            {
                _logger.Information("Start processing shipping request");

                using (_metrics.TimeOperation("shipping_info", "shipping_service"))
                {
                    var result = await _shippingHandler.Handle(request.Distance, request.IsExpress);

                    _metrics.IncrementOperation("shipping_info_success_count", "shipping_service");

                    _logger.Information("Processing shipping request completed");

                    return Ok(new ShippingResponse {Shipment = result});
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "An error occured while processing shipping request");
                _metrics.IncrementOperation("shipping_info_error_count", "shipping_service");
                return BadRequest();
            }
        }
    }
}