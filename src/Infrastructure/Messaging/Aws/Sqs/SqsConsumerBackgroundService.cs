using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using App.Metrics;
using Infrastructure.Instrumentation.Metrics;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Online.Store.Common.Commands;
using Serilog;

namespace Infrastructure.Messaging.Aws.Sqs
{
    public class SqsConsumerBackgroundService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IMetrics _metrics;
        private readonly IAwsSqs _awsSqs;
        private readonly IMediator _mediator;

        public SqsConsumerBackgroundService(ILogger logger
            , IMetrics metrics
            , IAwsSqs awsSqs
            , IMediator mediator)
        {
            _logger = logger;
            _metrics = metrics;
            _awsSqs = awsSqs;
            _mediator = mediator;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Information("Starting Sqs Consumer in background");

            await Start(stoppingToken);

            _logger.Information("Stopping Sqs Consumer in background");
        }

        private async Task Start(CancellationToken stoppingToken)
        {
            _logger.Information("Starting polling queue");

            while (!stoppingToken.IsCancellationRequested)
            {
                var messages = await _awsSqs.ReceiveMessageAsync();

                if (messages.Any())
                {
                    Console.WriteLine($"{messages.Count} messages received");

                    foreach (var msg in messages)
                    {
                        await _mediator.Send(new ConsumeAccountingCommand
                        {
                            AccountingCommand = JsonConvert.DeserializeObject<AccountingCommand>(msg.Body)
                        }, stoppingToken);

                        _logger.Information("{MessageId} processed with success", msg.MessageId);
                        _metrics.IncrementOperation($"sqs_message_received_count", "amazon_sqs");

                        await _awsSqs.HandleReceiptMessageAsync(msg.ReceiptHandle);
                    }
                }
                else
                {
                    _logger.Debug("No message available");
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                }
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Warning("The consumer background service is being stopped");
            return base.StopAsync(cancellationToken);
        }
    }
}