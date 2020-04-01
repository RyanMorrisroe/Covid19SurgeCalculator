using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using CovidSurgeCalculator.ModelData.Inputs;

namespace CovidSurgeCalculator.Site.Controllers
{
    public class InputController : Controller
    {
        private readonly ILogger<InputController> _logger;
        private readonly IMemoryCache _memoryCache;

        public InputController(ILogger<InputController> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public IActionResult Edit()
        {
            CalculatorInput inputs = (CalculatorInput)_memoryCache.Get("inputs");
            return View(inputs);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CalculatorInput inputs)
        {
            Contract.Requires(inputs != null);

            if(ModelState.IsValid)
            {
                _memoryCache.Set("inputs", inputs, new MemoryCacheEntryOptions() { Priority = CacheItemPriority.NeverRemove });
                await inputs.WriteBinaryToDisk(Path.Combine(Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "Binaries"), "Inputs.bin")).ConfigureAwait(true);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}