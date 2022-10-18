using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.SQS.Model;

namespace Infrastructure.Messaging.Aws.Sqs
{
    public interface IAwsSqs
    {
        Task<bool> SendMessageAsync<T>(T command);

        Task<List<Message>> ReceiveMessageAsync();

        Task HandleReceiptMessageAsync(string receiptHandleId);
    }
}