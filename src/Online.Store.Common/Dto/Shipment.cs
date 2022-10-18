namespace Online.Store.Common.Dto
{
    public class Shipment
    {
        public string ShippingMethod { get; set; }

        public string CompanyName { get; set; }

        public decimal Cost { get; set; }

        public int EstimatedDays { get; set; }
    }
}