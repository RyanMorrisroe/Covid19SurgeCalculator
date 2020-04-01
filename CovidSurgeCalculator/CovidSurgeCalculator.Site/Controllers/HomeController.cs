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
            _infectionModel = (ReferenceInfectionModel)memoryCache.Get("infectionModel");
        }

        private CalculatorInput GetInputs(HttpContext context)
        {
            if (_useSession)
            {
                if (!context.Session.TryGetValue("inputs", out _))
                {
                    CalculatorInput inputs = CalculatorInput.ReadBinaryFromDisk(Path.Combine(Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "Binaries"), "DefaultInputs.bin")).Result;
                    context.Session.SetString("inputs", JsonConvert.SerializeObject(inputs));
                    return inputs;
                }
                else
                {
                    return JsonConvert.DeserializeObject<CalculatorInput>(context.Session.GetString("inputs"));
                }
            }
            else
            {
                return (CalculatorInput)_memoryCache.Get("inputs");
            }
        }

        [HttpGet]
        public IActionResult WeeklyData()
        {
            ForecastModel forecast = new ForecastModel(GetInputs(HttpContext), _infectionModel);
            forecast.GridData = forecast.BuildGridDataFromWeeks(HttpContext, Request);
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
            forecast.GridData = forecast.BuildGridDataFromWeeks(HttpContext, Request);
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
            forecast.GridData = forecast.BuildGridDataFromWeeks(HttpContext, Request);
            return View(forecast);
        }

        [HttpGet]
        public IActionResult DailyData()
        {
            ForecastModel forecast = new ForecastModel(GetInputs(HttpContext), _infectionModel);
            forecast.GridData = forecast.BuildGridDataFromDays(HttpContext, Request);
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
            forecast.GridData = forecast.BuildGridDataFromDays(HttpContext, Request);
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
            forecast.GridData = forecast.BuildGridDataFromDays(HttpContext, Request);
            return View(forecast);
        }

        [HttpGet]
        public IActionResult Index()
        {
            ForecastModel forecast = new ForecastModel(GetInputs(HttpContext), _infectionModel);
            forecast.GridData = forecast.BuildGridDataFromWeeks(HttpContext, Request);
            return View(forecast);
        }

        [HttpGet]
        public IActionResult DailySurgeComparison()
        {
            ForecastModel forecast = new ForecastModel(GetInputs(HttpContext), _infectionModel);
            forecast.GridData = forecast.BuildGridDataFromWeeks(HttpContext, Request);
            return View(forecast);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
