using System.Collections.Generic;
using System.Threading.Tasks;
using Online.Store.Common.Dto;

namespace Inventory.Service.Application
{
    public interface IInventoryHandler
    {
        Task<List<Product>> GetAllProducts();

        Task AdjustInventory(string productId, int quantity);
    }
}