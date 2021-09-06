using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.QuickPulse;
using Microsoft.ApplicationInsights.Extensibility.EventCounterCollector;
using Microsoft.ApplicationInsights.DependencyCollector;
using System.Linq;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector;
using Microsoft.ApplicationInsights.AspNetCore;

namespace webapi_azure_oci
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Microsoft.ApplicationInsights.AspNetCore.Extensions.ApplicationInsightsServiceOptions aiOptions
            //    = new Microsoft.ApplicationInsights.AspNetCore.Extensions.ApplicationInsightsServiceOptions
            //    {
            //        EnableDependencyTrackingTelemetryModule = true,
            //        EnablePerformanceCounterCollectionModule = true,
            //        EnableRequestTrackingTelemetryModule = true
            //    };

            //services.AddApplicationInsightsTelemetry(aiOptions);

            //services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) =>
            //{
            //    module.EnableRequestIdHeaderInjectionInW3CMode = true;
            //    module.EnableSqlCommandTextInstrumentation = true;
            //});

            //// The following removes PerformanceCollectorModule to disable perf-counter collection.
            //// Similarly, any other default modules can be removed.
            //var performanceCounterService = services.FirstOrDefault<ServiceDescriptor>(t => t.ImplementationType == typeof(PerformanceCollectorModule));
            //if (performanceCounterService != null)
            //{
            //    services.Remove(performanceCounterService);
            //}
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
