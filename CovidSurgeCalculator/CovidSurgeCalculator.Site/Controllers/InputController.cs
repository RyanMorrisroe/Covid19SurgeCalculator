using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CovidSurgeCalculator.ModelData.Inputs;
using CovidSurgeCalculator.ModelData.ReferenceData;

namespace CovidSurgeCalculator.Site.Controllers
{
    public class InputController : Controller
    {
        private readonly ILogger<InputController> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly bool _useSession;

        public InputController(ILogger<InputController> logger, IMemoryCache memoryCache, IConfiguration configuration)
        {
            Contract.Requires(configuration != null);

            _logger = logger;
            _memoryCache = memoryCache;
            _useSession = bool.Parse(configuration["UseSession"]);
        }

        [HttpGet]
        public IActionResult Edit()
        {
            CalculatorInput inputs;
            if(_useSession)
            {
                _logger.LogInformation("Pulling inputs from session");
                inputs = JsonConvert.DeserializeObject<CalculatorInput>(HttpContext.Session.GetString("inputs"));
            }
            else
            {
                _logger.LogInformation("Pulling inputs from memory cache");
                inputs = (CalculatorInput)_memoryCache.Get("inputs");
            }    
            return View(inputs);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CalculatorInput inputs)
        {
            Contract.Requires(inputs != null);

            if(ModelState.IsValid)
            {
                if(_useSession)
                {
                    HttpContext.Session.SetString("inputs", JsonConvert.SerializeObject(inputs));
                    _logger.LogInformation("Wrote inputs to session");
                }
                else
                {
                    _memoryCache.Set("inputs", inputs, new MemoryCacheEntryOptions() { Priority = CacheItemPriority.NeverRemove });
                    _logger.LogInformation("Wrote inputs to memory cache");
                    await inputs.WriteBinaryToDisk(Path.Combine(Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "Binaries"), "Inputs.bin")).ConfigureAwait(true);
                    _logger.LogInformation("Wrote inputs to disk");
                }
                ReferenceInfectionModel infectionModel = (ReferenceInfectionModel)_memoryCache.Get("infectionModel");
                _logger.LogInformation("Pulled infection model from memory cache");
                infectionModel.ApplyCalculatorInputToAgeCohortInformation(inputs);
                _logger.LogInformation("Reapplied inputs to infection model");
                _memoryCache.Set("infectionModel", infectionModel);
                _logger.LogInformation("Stored infection model in memory cache");
            }
            return RedirectToAction("Index", "Home");
        }
    }
}