using System.Collections.Generic;
using System.Threading.Tasks;
using Online.Store.Common.Dto;
using Refit;

namespace Infrastructure.Sdk.Api
{
    public interface IInventoryServiceApi
    {
        [Get("/products")]
        Task<List<Product>> Products();

        [Post("/adjust")]
        Task Adjust(InventoryRequest request);
    }
}