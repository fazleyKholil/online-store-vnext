namespace Infrastructure.Messaging.Aws.Sqs
{
    public class SqsOptions
    {
        public string QueueUrl { get; set; }

        public string ServiceUrl { get; set; }

        public string AccessKey { get; set; }

        public string SecretKey { get; set; }
    }
}