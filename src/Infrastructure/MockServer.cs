using System;
using System.Threading.Tasks;
using Online.Store.Common.Dto;

namespace Infrastructure
{
    public static class MockServer
    {
        public static async Task<Shipment> SendShippingRequest(string shipping, int distance)
        {
            switch (shipping)
            {
                case "Dhl":
                    await Task.Delay(ExternalServicesConst.DhlResponseTime);
                    return new Shipment
                    {
                        Cost = distance * 0.3M, //usd per km
                        CompanyName = "DHL",
                        EstimatedDays = new Random().Next(1, 10),
                        ShippingMethod = "Air"
                    };

                case "Fedex":
                    await Task.Delay(ExternalServicesConst.FedexResponseTime);
                    return new Shipment
                    {
                        Cost = distance * 0.2M, //usd per km
                        CompanyName = "Fedex",
                        EstimatedDays = new Random().Next(4, 14),
                        ShippingMethod = "Air"
                    };

                case "Aramex":
                    await Task.Delay(ExternalServicesConst.AramexResponseTime);
                    return new Shipment
                    {
                        Cost = distance * 0.1M, // usd per km
                        CompanyName = "Aramex",
                        EstimatedDays = new Random().Next(4, 14),
                        ShippingMethod = "Air"
                    };

                default:
                    return new Shipment
                    {
                        Cost = distance * 0.1M, // usd per km
                        CompanyName = "Aramex",
                        EstimatedDays = new Random().Next(4, 14),
                        ShippingMethod = "Air"
                    };
            }
        }
    }
}