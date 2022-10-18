using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Sdk
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOnlineStoreSdk(this IServiceCollection services, IConfiguration configuration)
        {
            var options = new MicroserviceApiOptions();
            configuration.GetSection("Microservice").Bind(options);
            services.AddSingleton(options);

            services.AddSingleton<IOnlineStoreSdk, OnlineStoreSdk>();

            return services;
        }
    }
}