using System.Threading.Tasks;
using Online.Store.Common.Dto;
using Refit;

namespace Infrastructure.Sdk.Api
{
    public interface IShippingServiceApi
    {
        [Post("/info")]
        Task<ShippingResponse> Info(ShippingRequest request);
    }
}