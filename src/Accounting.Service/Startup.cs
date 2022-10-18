using System;
using Accounting.Service.Application;
using Accounting.Service.Persistence;
using App.Metrics;
using Infrastructure.Instrumentation.Metrics;
using Infrastructure.Messaging.Aws.Sqs;
using Infrastructure.Utils;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Online.Store.Common.Dto;
using Serilog;

namespace Accounting.Service
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
        public Serilog.ILogger Logger { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Logger);
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "Accounting.Service", Version = "v1"}); });

            services.AddMetrics(Configuration, ReflectionUtils.GetAssemblyVersion<Program>());
            services.AddAmazonSqs(Configuration);
            services.AddSingleton<ILedgerDb, LedgerDb>();
            services.AddMediatR(typeof(Startup));
            services.AddControllers();

            services.AddSqsBackgroundConsumer();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMetrics metrics)
        {
            metrics.IncrementOperation("accounting_service_count");

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
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Accounting.Service v1"));
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