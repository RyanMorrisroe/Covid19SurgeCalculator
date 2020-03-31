using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Threading.Tasks;
using CovidSurgeCalculator.ModelData.Inputs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CovidSurgeCalculator.Site.Controllers
{
    public class InputController : Controller
    {
        private readonly ILogger<InputController> _logger;
        private readonly string _inputsPath;

        public InputController(ILogger<InputController> logger)
        {
            _logger = logger;
            _inputsPath = Path.Combine(Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "Binaries"), "Inputs.bin");
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            CalculatorInput inputs = await CalculatorInput.ReadBinaryFromDisk(_inputsPath).ConfigureAwait(true);
            return View(inputs);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CalculatorInput inputs)
        {
            Contract.Requires(inputs != null);

            if(ModelState.IsValid)
            {
                await inputs.WriteBinaryToDisk(_inputsPath).ConfigureAwait(true);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}