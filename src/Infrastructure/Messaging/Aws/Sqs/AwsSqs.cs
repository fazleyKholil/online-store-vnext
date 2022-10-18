using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using App.Metrics;
using Infrastructure.Instrumentation.Metrics;
using Infrastructure.Resiliency;
using Newtonsoft.Json;
using Serilog;

namespace Infrastructure.Messaging.Aws.Sqs
{
    public class AwsSqs : IAwsSqs
    {
        private readonly ILogger _logger;
        private readonly IMetrics _metrics;
        private readonly IAmazonSQS _sqs;
        private readonly SqsOptions _options;

        public AwsSqs(ILogger logger
            , IMetrics metrics
            , IAmazonSQS sqs
            , SqsOptions options)
        {
            _logger = logger;
            _metrics = metrics;
            _sqs = sqs;
            _options = options;
        }

        public async Task<bool> SendMessageAsync<T>(T command)
        {
            try
            {
                _metrics.IncrementOperation($"sqs_message_send_count", "amazon_sqs");
                var message = string.Empty;
                using (_metrics.TimeOperation("serialize_message", "amazon_sqs"))
                {
                    _logger.Information("Serializing object before sending to Sqs");
                    message = JsonConvert.SerializeObject(command);
                }

                using (_metrics.TimeOperation("sending_sqs_message", "amazon_sqs"))
                {
                    _logger.Information("Sending message to Sqs");
                    var sendRequest = new SendMessageRequest(_options.QueueUrl, message);

                    var sendResult = await _sqs.SendMessageAsync(sendRequest);

                    if (sendResult.HttpStatusCode == System.Net.HttpStatusCode.OK)
                        _logger.Information("Message successfully send");
                    else
                        _logger.Warning("Message was not send with HttpStatusCode {HttStatusCode}", sendResult.HttpStatusCode);

                    return sendResult.HttpStatusCode == System.Net.HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occured while sending message to Sqs");
                throw;
            }
        }

        public async Task<List<Message>> ReceiveMessageAsync()
        {
            try
            {
                var request = new ReceiveMessageRequest
                {
                    QueueUrl = _options.QueueUrl,
                    MaxNumberOfMessages = 10,
                    WaitTimeSeconds = 1
                };

                using (_metrics.TimeOperation("polling_sqs", "amazon_sqs"))
                {
                    ReceiveMessageResponse result = null;
                    await PollyRetryRegistry.GetPolicyAsync(20, 2, "ReceiveMessageAsync", _logger)
                        .ExecuteAsync(async () => { result = await _sqs.ReceiveMessageAsync(request); });

                    return result.Messages.Any() ? result.Messages : new List<Message>();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occured while receiving Sqs messages");
                throw;
            }
        }

        public async Task HandleReceiptMessageAsync(string receiptHandleId)
        {
            var request = new DeleteMessageRequest
            {
                QueueUrl = _options.QueueUrl,
                ReceiptHandle = receiptHandleId
            };

            await _sqs.DeleteMessageAsync(request);
        }
    }
}