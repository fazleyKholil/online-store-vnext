using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Metrics;
using Infrastructure.Instrumentation.Metrics;
using Infrastructure.Messaging.Aws.Sqs;
using Infrastructure.Sdk;
using Infrastructure.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Online.Store.Api.Application;
using Online.Store.Common.Dto;
using Serilog;

namespace Online.Store.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.WithProperty("Version", ReflectionUtils.GetAssemblyVersion<Startup>())
                .CreateLogger();
        }

        public IConfiguration Configuration { get; }
        public ILogger Logger { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Logger);
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "OnlineStore.Api", Version = "v1"}); });

            services.AddMetrics(Configuration, ReflectionUtils.GetAssemblyVersion<Program>());
            services.AddAmazonSqs(Configuration);
            services.AddOnlineStoreSdk(Configuration);
            services.AddSingleton<IProcessOrderHandler, ProcessOrderHandler>();

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMetrics metrics)
        {
            metrics.IncrementOperation("online_store_api_count");

            var serverId = Guid.NewGuid().ToString();
            var pathBase = Configuration.GetValue<string>("ASPNETCORE_PATHBASE");
            if (!string.IsNullOrWhiteSpace(pathBase))
            {
                app.UsePathBase(pathBase);
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OnlineStore.Api vnext"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/_system/health", new HealthCheckOptions
                {
                    ResponseWriter = HealthReports.FormatAsync
                });

                endpoints.MapGet("/_system/id", async context => { await context.Response.WriteAsync($"ServerId : {serverId}"); });
            });

            app.UseMetrics();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}