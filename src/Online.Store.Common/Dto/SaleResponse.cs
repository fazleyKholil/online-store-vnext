using System.Collections.Generic;

namespace Online.Store.Common.Dto
{
    public class SaleResponse
    {
        public decimal Total { get; set; }

        public List<object> Products { get; set; }

        public Shipment ShippingInformation { get; set; }
    }
}