using System.Threading.Tasks;
using Online.Store.Common.Dto;
using Shipping.Service.Domain;

namespace Shipping.Service.Application
{
    public interface IShippingHandler
    {
        Task<Shipment> Handle(int distance, bool isExpress);
    }
}