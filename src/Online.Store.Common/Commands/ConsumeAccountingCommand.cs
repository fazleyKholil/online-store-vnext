using MediatR;

namespace Online.Store.Common.Commands
{
    public class ConsumeAccountingCommand : IRequest
    {
        public AccountingCommand AccountingCommand { get; set; }
    }
}