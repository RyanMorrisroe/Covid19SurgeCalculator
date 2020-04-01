using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using CovidSurgeCalculator.ModelData.Inputs;
using CovidSurgeCalculator.ModelData.ReferenceData;
using CovidSurgeCalculator.Site.Models;

namespace CovidSurgeCalculator.Site.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly ReferenceInfectionModel _infectionModel;
        private readonly bool _useSession;

        public HomeController(ILogger<HomeController> logger, IMemoryCache memoryCache, IConfiguration configuration)
        {
            Contract.Requires(configuration != null);

            _logger = logger;
            _memoryCache = memoryCache;
            _useSession = bool.Parse(configuration["UseSession"]);
            _logger.LogInformation("Retriving infection model from cache");
            _infectionModel = (ReferenceInfectionModel)memoryCache.Get("infectionModel");
        }

        private CalculatorInput GetInputs(HttpContext context)
        {
            if (_useSession)
            {
                if (!context.Session.TryGetValue("inputs", out _))
                {
                    _logger.LogInformation("Inputs not found in session, retrieving defaults from disk");
                    CalculatorInput inputs = CalculatorInput.ReadBinaryFromDisk(Path.Combine(Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "Binaries"), "DefaultInputs.bin")).Result;
                    _logger.LogInformation("Default inputs read from disk");
                    context.Session.SetString("inputs", JsonConvert.SerializeObject(inputs));
                    _logger.LogInformation("Default inputs stored in session");
                    return inputs;
                }
                else
                {
                    _logger.LogInformation("Inputs found in session");
                    return JsonConvert.DeserializeObject<CalculatorInput>(context.Session.GetString("inputs"));
                }
            }
            else
            {
                _logger.LogInformation("Retrieving inputs from cache");
                return (CalculatorInput)_memoryCache.Get("inputs");
            }
        }

        [HttpGet]
        public IActionResult WeeklyData()
        {
            ForecastModel forecast = new ForecastModel(GetInputs(HttpContext), _infectionModel);
            _logger.LogInformation("Built new weekly forecast");
            forecast.GridData = forecast.BuildGridDataFromWeeks(HttpContext, Request);
            _logger.LogInformation("Built grid from weekly forecast");
            return View(forecast);
        }

        [HttpGet]
        public IActionResult WeeklyDataExport(ForecastModel modifiedForecast)
        {
            Contract.Requires(modifiedForecast != null);

            ForecastModel forecast = new ForecastModel(GetInputs(HttpContext), _infectionModel)
            {
                SelectedForecast = modifiedForecast.SelectedForecast
            };
            _logger.LogInformation("Built new weekly forecast from existing");
            forecast.GridData = forecast.BuildGridDataFromWeeks(HttpContext, Request);
            _logger.LogInformation("Built grid from new weekly forecast");
            return forecast.GridExport();
        }

        [HttpPost]
        public IActionResult WeeklyData(ForecastModel modifiedForecast)
        {
            Contract.Requires(modifiedForecast != null);

            ForecastModel forecast = new ForecastModel(GetInputs(HttpContext), _infectionModel)
            {
                SelectedForecast = modifiedForecast.SelectedForecast
            };
            _logger.LogInformation("Built new weekly forecast from existing");
            forecast.GridData = forecast.BuildGridDataFromWeeks(HttpContext, Request);
            _logger.LogInformation("Built grid from new weekly forecast");
            return View(forecast);
        }

        [HttpGet]
        public IActionResult DailyData()
        {
            ForecastModel forecast = new ForecastModel(GetInputs(HttpContext), _infectionModel);
            _logger.LogInformation("Built new daily forecast");
            forecast.GridData = forecast.BuildGridDataFromDays(HttpContext, Request);
            _logger.LogInformation("Built grid from daily forecast");
            return View(forecast);
        }

        [HttpGet]
        public IActionResult DailyDataExport(ForecastModel modifiedForecast)
        {
            Contract.Requires(modifiedForecast != null);

            ForecastModel forecast = new ForecastModel(GetInputs(HttpContext), _infectionModel)
            {
                SelectedForecast = modifiedForecast.SelectedForecast
            };
            _logger.LogInformation("Built new daily forecast from existing");
            forecast.GridData = forecast.BuildGridDataFromDays(HttpContext, Request);
            _logger.LogInformation("Built grid from new daily forecast");
            return forecast.GridExport();
        }

        [HttpPost]
        public IActionResult DailyData(ForecastModel modifiedForecast)
        {
            Contract.Requires(modifiedForecast != null);

            ForecastModel forecast = new ForecastModel(GetInputs(HttpContext), _infectionModel)
            {
                SelectedForecast = modifiedForecast.SelectedForecast
            };
            _logger.LogInformation("Built new daily forecast from existing");
            forecast.GridData = forecast.BuildGridDataFromDays(HttpContext, Request);
            _logger.LogInformation("Built grid from new daily forecast");
            return View(forecast);
        }

        [HttpGet]
        public IActionResult Index()
        {
            ForecastModel forecast = new ForecastModel(GetInputs(HttpContext), _infectionModel);
            _logger.LogInformation("Built new weekly forecast");
            return View(forecast);
        }

        [HttpGet]
        public IActionResult DailySurgeComparison()
        {
            ForecastModel forecast = new ForecastModel(GetInputs(HttpContext), _infectionModel);
            _logger.LogInformation("Built new daily forecast");
            return View(forecast);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
