using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Metrics;
using Infrastructure.Instrumentation.Metrics;
using Infrastructure.Utils;
using Inventory.Service.Application;
using Inventory.Service.Persistence;
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
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Inventory.Service
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
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "Inventory.Service", Version = "v1"}); });

            services.AddMetrics(Configuration, ReflectionUtils.GetAssemblyVersion<Program>());

            services.AddSingleton<IInventoryHandler, InventoryHandler>();
            services.AddSingleton<IInventoryDb, InventoryDb>();

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMetrics metrics)
        {
            metrics.IncrementOperation("inventory_service_count");

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
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Inventory.Service v1"));
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