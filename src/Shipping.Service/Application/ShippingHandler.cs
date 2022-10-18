using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Metrics;
using Infrastructure.Resiliency;
using Online.Store.Common.Dto;
using Serilog;
using Shipping.Service.Domain;

namespace Shipping.Service.Application
{
    public class ShippingHandler : IShippingHandler
    {
        private readonly IEnumerable<IShippingMethod> _shippingMethods;
        private readonly ILogger _logger;
        private readonly IMetrics _metrics;

        public ShippingHandler(ILogger logger
            , IMetrics metrics
            , IEnumerable<IShippingMethod> shippingMethods)
        {
            _logger = logger;
            _metrics = metrics;
            _shippingMethods = shippingMethods;
        }

        public async Task<Shipment> Handle(int distance, bool isExpress)
        {
            _logger.Information("Start to compare shipping methods");

            var shippingOptions = new List<Shipment>();

            foreach (var shippingMethod in _shippingMethods)
            {
                _logger.Information("Requesting {ShippingMethod} shipping details", shippingMethod.Name);

                await PollyRetryRegistry.GetPolicyAsync(5, 2, "RequestShippingMethod", _logger)
                    .ExecuteAsync(async () =>
                    {
                        var response = await shippingMethod.Get(distance, isExpress);
                        shippingOptions.Add(response);
                    });

                _logger.Information("Response received from {ShippingMethod}", shippingMethod.Name);
            }

            _logger.Information("Calculating the best shipping offer");

            var bestCost = shippingOptions.Min(s => s.Cost);
            var bestTimeArrival = shippingOptions.Min(s => s.EstimatedDays);

            return isExpress
                ? shippingOptions.FirstOrDefault(s => s.EstimatedDays == bestTimeArrival)
                : shippingOptions.FirstOrDefault(s => s.Cost == bestCost);
        }
    }
}