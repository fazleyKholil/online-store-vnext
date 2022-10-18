namespace Online.Store.Common.Dto
{
    public class Product
    {
        public string ProductId { get; set; }

        public string Description { get; set; }

        public decimal BuyingPrice { get; set; }

        public decimal SellingPrice { get; set; }

        public int Quantity { get; set; }
    }
}