using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CovidSurgeCalculator.ModelData.Inputs;
using CovidSurgeCalculator.ModelData.ReferenceData;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CovidSurgeCalculator.Site
{
    public sealed class ModelCacheHostedService : IHostedService
    {
        private readonly IMemoryCache memoryCache;
        private readonly IWebHostEnvironment environment;
        private readonly IConfiguration configuration;
        private readonly ILogger<ModelCacheHostedService> logger;

        public ModelCacheHostedService(
            IMemoryCache memoryCache,
            IWebHostEnvironment environment,
            IConfiguration configuration,
            ILogger<ModelCacheHostedService> logger)
        {
            this.memoryCache = memoryCache;
            this.environment = environment;
            this.configuration = configuration;
            this.logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            string binaryPath = Path.Combine(environment.ContentRootPath, "App_Data", "Binaries");
            bool useSession = configuration.GetValue<bool>("UseSession");
            string inputFileName = useSession ? "DefaultInputs.bin" : "Inputs.bin";

            CalculatorInput inputs = await CalculatorInput.ReadBinaryFromDisk(
                Path.Combine(binaryPath, inputFileName)).ConfigureAwait(false);
            logger.LogInformation("Inputs read from disk");

            ReferenceInfectionModel infectionModel = await ReferenceInfectionModel.ReadBinaryFromDisk(
                Path.Combine(binaryPath, "ReferenceInfectionModel.bin"), inputs).ConfigureAwait(false);
            logger.LogInformation("Infection model read from disk");

            MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions
            {
                Priority = CacheItemPriority.NeverRemove
            };

            if (useSession)
            {
                memoryCache.Set("defaultInputs", inputs, cacheOptions);
            }
            else
            {
                memoryCache.Set("inputs", inputs, cacheOptions);
            }

            memoryCache.Set("infectionModel", infectionModel, cacheOptions);
            logger.LogInformation("Model data added to memory cache");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
