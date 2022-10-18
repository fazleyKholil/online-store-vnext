using System.Collections.Generic;

namespace Online.Store.Common.Dto
{
    public class OrderRequest
    {
        public List<Product> Products { get; set; }

        public bool IsShippingExpress { get; set; }

        //for demo simplicity to calculate shipping cost
        public int ShippingDistance { get; set; }
    }
}