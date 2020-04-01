using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CovidSurgeCalculator.ModelData.Inputs;
using CovidSurgeCalculator.ModelData.ReferenceData;

namespace CovidSurgeCalculator.Site
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddMemoryCache();
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger, IMemoryCache memoryCache)
        {
            Contract.Requires(env != null);
            Contract.Requires(memoryCache != null);

            if (env.IsDevelopment())
            {
                logger.LogInformation("In development environment");
                app.UseDeveloperExceptionPage();
            }
            else
            {
                logger.LogInformation("In production");
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            string dataDirectory = Path.Combine(env.ContentRootPath, "App_Data");
            AppDomain.CurrentDomain.SetData("DataDirectory", dataDirectory);
            LoadCache(logger, memoryCache, dataDirectory).Wait();
        }

        private static async Task LoadCache(ILogger<Startup> logger, IMemoryCache memoryCache, string dataDirectory)
        {
            string binaryPath = Path.Combine(dataDirectory, "Binaries");
            CalculatorInput inputs = await CalculatorInput.ReadBinaryFromDisk(Path.Combine(binaryPath, "Inputs.bin")).ConfigureAwait(true);
            logger.LogInformation("Inputs read from disk");
            ReferenceInfectionModel infectionModel = await ReferenceInfectionModel.ReadBinaryFromDisk(Path.Combine(binaryPath, "ReferenceInfectionModel.bin"), inputs).ConfigureAwait(true);
            logger.LogInformation("Infection Model read from disk");

            MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions()
            {
                Priority = CacheItemPriority.NeverRemove
            };

            if (!memoryCache.TryGetValue("inputs", out _))
            {
                memoryCache.Set("inputs", inputs, cacheOptions);
                logger.LogInformation("Added inputs to cache");
            }
            if (!memoryCache.TryGetValue("infectionModel", out _))
            {
                memoryCache.Set("infectionModel", infectionModel, cacheOptions);
                logger.LogInformation("Added infection model to cache");
            }
        }
    }
}
