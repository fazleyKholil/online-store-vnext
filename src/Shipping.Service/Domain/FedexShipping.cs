using System.Threading.Tasks;
using App.Metrics;
using Infrastructure;
using Infrastructure.Instrumentation.Metrics;
using Online.Store.Common.Dto;
using Serilog;

namespace Shipping.Service.Domain
{
    public class FedexShipping : IShippingMethod
    {
        private readonly ILogger _logger;
        private readonly IMetrics _metrics;

        public string Name { get; set; }

        public FedexShipping(ILogger logger, IMetrics metrics)
        {
            _logger = logger;
            _metrics = metrics;
            Name = "Fedex";
        }

        public async Task<Shipment> Get(int distance, bool isExpress)
        {
            _logger.Information("Sending request to Fedex Shipping Service");

            using (_metrics.TimeOperation("request_external_shipping_service_fedex", "shipping_service"))
            {
                //Fedex specific logic and integration to the external api

                //simulating Fedex external service
                return await MockServer.SendShippingRequest("Fedex", distance);
            }
        }
    }
}