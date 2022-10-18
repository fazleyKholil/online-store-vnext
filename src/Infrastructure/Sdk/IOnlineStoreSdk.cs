using System.Collections.Generic;
using System.Threading.Tasks;
using Online.Store.Common.Commands;
using Online.Store.Common.Dto;

namespace Infrastructure.Sdk
{
    public interface IOnlineStoreSdk
    {
        Task<List<Product>> GetProducts();

        Task AdjustInventory(InventoryRequest request);

        Task<ShippingResponse> ShippingInformation(ShippingRequest request);

        Task PerformAccounting(AccountingCommand command);
    }
}