using System;
using System.Threading.Tasks;
using App.Metrics;
using Infrastructure.Messaging.Aws.Sqs;
using Microsoft.AspNetCore.Mvc;
using Online.Store.Common.Commands;
using Serilog;

namespace Accounting.Service.Application
{
    [ApiController]
    [Route("[controller]")]
    public class AccountingController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMetrics _metrics;
        private readonly IAwsSqs _awsSqs;

        public AccountingController(ILogger logger
            , IMetrics metrics
            , IAwsSqs awsSqs)
        {
            _logger = logger;
            _metrics = metrics;
            _awsSqs = awsSqs;
        }

        [HttpPost]
        [Route("send")]
        public async Task<IActionResult> Send(AccountingCommand command)
        {
            try
            {
                var result = await _awsSqs.SendMessageAsync(command);

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.Error(e, "An error occured while sending message");
                return BadRequest(e);
            }
        }
    }
}