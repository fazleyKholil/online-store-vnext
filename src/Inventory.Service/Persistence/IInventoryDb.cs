using System.Collections.Generic;
using System.Threading.Tasks;
using Online.Store.Common.Dto;

namespace Inventory.Service.Persistence
{
    public interface IInventoryDb
    {
        Task<List<Product>> GetAll();

        Task UpdateStock(string productId, int quantity);
    }
}