using Amazon.Runtime;
using Amazon.SQS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Messaging.Aws.Sqs
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAmazonSqs(this IServiceCollection services, IConfiguration configuration)
        {
            var sqsOptions = new SqsOptions();
            configuration.GetSection("Sqs").Bind(sqsOptions);
            services.AddSingleton(sqsOptions);

            IAmazonSQS sqsClient;

            if (string.IsNullOrWhiteSpace(sqsOptions.ServiceUrl))
            {
                var awsOptions = configuration.GetAWSOptions();
                sqsClient = awsOptions.CreateServiceClient<IAmazonSQS>();
            }
            else
            {
                var credentials = new BasicAWSCredentials(sqsOptions.AccessKey, sqsOptions.SecretKey);
                var config = new AmazonSQSConfig
                {
                    ServiceURL = sqsOptions.ServiceUrl
                };

                sqsClient = new AmazonSQSClient(credentials, config);
            }

            services.AddSingleton(sqsClient);
            services.AddSingleton<IAwsSqs, AwsSqs>();

            return services;
        }

        public static IServiceCollection AddSqsBackgroundConsumer(this IServiceCollection services)
        {
            services.AddHostedService<SqsConsumerBackgroundService>();
            return services;
        }
    }
}