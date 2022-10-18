using System.Threading.Tasks;
using Online.Store.Common.Dto;

namespace Online.Store.Api.Application
{
    public interface IProcessOrderHandler
    {
        Task<SaleResponse> Handle(OrderRequest request);
    }
}