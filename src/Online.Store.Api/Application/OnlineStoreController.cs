using System;
using System.Threading.Tasks;
using App.Metrics;
using Infrastructure.Instrumentation.Metrics;
using Microsoft.AspNetCore.Mvc;
using Online.Store.Common.Dto;
using Serilog;

namespace Online.Store.Api.Application
{
    [ApiController]
    [Route("[controller]")]
    public class OnlineStoreController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMetrics _metrics;
        private readonly IProcessOrderHandler _handler;

        public OnlineStoreController(ILogger logger
            , IMetrics metrics
            , IProcessOrderHandler handler)
        {
            _logger = logger;
            _metrics = metrics;
            _handler = handler;
        }

        [HttpPost]
        [Route("order")]
        public async Task<IActionResult> Info(OrderRequest request)
        {
            _metrics.IncrementOperation("order_request_count", "online_api");

            try
            {
                _logger.Information("Start processing order");

                using (_metrics.TimeOperation("process_order", "online_api"))
                {
                    var result = await _handler.Handle(request);

                    _metrics.IncrementOperation("order_success_count", "online_api");

                    _logger.Information("Processing order completed");

                    return Ok(result);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "An error occured while processing the order");
                _metrics.IncrementOperation("order_error_count", "online_api");
                return BadRequest();
            }
        }
    }
}