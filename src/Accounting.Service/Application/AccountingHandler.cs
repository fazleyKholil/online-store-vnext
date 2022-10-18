using System;
using System.Threading;
using System.Threading.Tasks;
using Accounting.Service.Persistence;
using App.Metrics;
using Infrastructure.Instrumentation.Metrics;
using Infrastructure.Messaging.Aws.Sqs;
using MediatR;
using Online.Store.Common.Commands;
using Serilog;
using Unit = MediatR.Unit;

namespace Accounting.Service.Application
{
    public class AccountingHandler : IRequestHandler<ConsumeAccountingCommand>
    {
        private readonly ILogger _logger;
        private readonly IMetrics _metrics;
        private readonly ILedgerDb _ledgerDb;

        public AccountingHandler(ILogger logger
            , IMetrics metrics
            , ILedgerDb ledgerDb)
        {
            _logger = logger;
            _metrics = metrics;
            _ledgerDb = ledgerDb;
        }

        public async Task<Unit> Handle(ConsumeAccountingCommand request, CancellationToken cancellationToken)
        {
            _logger.Information("Starting accounting process for order");

            using (_metrics.TimeOperation("process_accounting", "accounting_service"))
            {
                //calculate profit
                var totalCost = 0M;
                var totalSales = 0M;
                var totalProfit = 0M;

                foreach (var product in request.AccountingCommand.Sale.Products)
                {
                    totalCost += product.BuyingPrice;
                    totalSales += product.SellingPrice;
                    totalProfit += (product.SellingPrice - product.BuyingPrice);
                }

                //calculate tax amount varies by product
                var tax = totalCost * (15M / 100M);
                var netProfit = totalProfit - tax;

                //apply to ledger assume it takes several process/steps for demo purposed
                //lock optimisation after load test to improve performance
                // await _ledgerDb.UpdateLedger(totalSales, totalCost, totalProfit, netProfit);
                await _ledgerDb.UpdateLedgerOptimised(totalSales, totalCost, totalProfit, netProfit);

                _logger.Information(" Accounting process for order completed");
            }

            return Unit.Value;
        }
    }
}