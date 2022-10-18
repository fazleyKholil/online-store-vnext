using System.Threading.Tasks;
using App.Metrics;
using Infrastructure;
using Infrastructure.Instrumentation.Metrics;
using Online.Store.Common.Dto;
using Serilog;

namespace Shipping.Service.Domain
{
    public class DhlShipping : IShippingMethod
    {
        private readonly ILogger _logger;
        private readonly IMetrics _metrics;

        public string Name { get; set; }

        public DhlShipping(ILogger logger, IMetrics metrics)
        {
            _logger = logger;
            _metrics = metrics;
            Name = "Dhl";
        }

        public async Task<Shipment> Get(int distance, bool isExpress)
        {
            _logger.Information("Sending request to Dhl Shipping Service");

            using (_metrics.TimeOperation("request_external_shipping_service_dhl", "shipping_service"))
            {
                //Dhl specific logic and integration to the external api

                //simulating Dhl external service
                return await MockServer.SendShippingRequest("Dhl", distance);
            }
        }
    }
}