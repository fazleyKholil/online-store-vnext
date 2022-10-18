using System.Collections.Generic;

namespace Online.Store.Common.Dto
{
    public class Sale
    {
        public string SaleId { get; set; }

        public List<Product> Products { get; set; }

        public decimal ShippingCost { get; set; }

        public Sale()
        {
            Products = new List<Product>();
        }
    }
}