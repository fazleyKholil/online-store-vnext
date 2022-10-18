using System.Threading.Tasks;
using Online.Store.Common.Dto;

namespace Shipping.Service.Domain
{
    public interface IShippingMethod
    {
        public string Name { get; set; }

        Task<Shipment> Get(int distance, bool isExpress);
    }
}