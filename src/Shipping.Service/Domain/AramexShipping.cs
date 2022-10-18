using System.Threading.Tasks;
using App.Metrics;
using Infrastructure;
using Infrastructure.Instrumentation.Metrics;
using Online.Store.Common.Dto;
using Serilog;

namespace Shipping.Service.Domain
{
    public class AramexShipping : IShippingMethod
    {
        private readonly ILogger _logger;
        private readonly IMetrics _metrics;

        public string Name { get; set; }

        public AramexShipping(ILogger logger, IMetrics metrics)
        {
            _logger = logger;
            _metrics = metrics;
            Name = "Aramex";
        }

        public async Task<Shipment> Get(int distance, bool isExpress)
        {
            _logger.Information("Sending request to Aramex Shipping Service");

            using (_metrics.TimeOperation("request_external_shipping_service_aramex", "shipping_service"))
            {
                //Aramex specific logic and integration to the external api

                //simulating Aramex external service
                return await MockServer.SendShippingRequest("Aramex", distance);
            }
        }
    }
}