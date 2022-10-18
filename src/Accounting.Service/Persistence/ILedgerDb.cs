using System.Threading.Tasks;
using Online.Store.Common.Dto;

namespace Accounting.Service.Persistence
{
    public interface ILedgerDb
    {
        Ledger GetLedger();

        Task<Ledger> UpdateLedgerOptimised(decimal totalSales, decimal totalCost, decimal totalProfit, decimal netProfit);
    }
}