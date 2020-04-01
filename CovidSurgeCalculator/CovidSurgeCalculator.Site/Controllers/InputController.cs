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
                inputs = JsonConvert.DeserializeObject<CalculatorInput>(HttpContext.Session.GetString("inputs"));
            }
            else
            {
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
                }
                else
                {
                    _memoryCache.Set("inputs", inputs, new MemoryCacheEntryOptions() { Priority = CacheItemPriority.NeverRemove });
                    await inputs.WriteBinaryToDisk(Path.Combine(Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "Binaries"), "Inputs.bin")).ConfigureAwait(true);
                }
                ReferenceInfectionModel infectionModel = (ReferenceInfectionModel)_memoryCache.Get("infectionModel");
                infectionModel.ApplyCalculatorInputToAgeCohortInformation(inputs);
                _memoryCache.Set("infectionModel", infectionModel);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}