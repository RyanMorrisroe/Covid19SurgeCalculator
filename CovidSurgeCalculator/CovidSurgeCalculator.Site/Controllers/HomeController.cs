﻿using System.Diagnostics;
using System.Diagnostics.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CovidSurgeCalculator.ModelData.Inputs;
using CovidSurgeCalculator.ModelData.ReferenceData;
using CovidSurgeCalculator.Site.Models;
using System.IO;
using System;

namespace CovidSurgeCalculator.Site.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CalculatorInput _inputs;
        private readonly ReferenceInfectionModel _infectionModel;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;

            string binaryPath = Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "Binaries");
            _inputs = CalculatorInput.ReadBinaryFromDisk(Path.Combine(binaryPath, "Inputs.bin")).Result;
            _infectionModel = ReferenceInfectionModel.ReadBinaryFromDisk(Path.Combine(binaryPath, "ReferenceInfectionModel.bin"), _inputs).Result;
        }

        [HttpGet]
        public IActionResult WeeklyData()
        {
            ForecastModel forecast = new ForecastModel(_inputs, _infectionModel);
            forecast.GridData = forecast.BuildGridDataFromWeeks(HttpContext, Request);
            return View(forecast);
        }

        [HttpGet]
        public IActionResult WeeklyDataExport(ForecastModel modifiedForecast)
        {
            Contract.Requires(modifiedForecast != null);

            ForecastModel forecast = new ForecastModel(_inputs, _infectionModel)
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

            ForecastModel forecast = new ForecastModel(_inputs, _infectionModel)
            {
                SelectedForecast = modifiedForecast.SelectedForecast
            };
            forecast.GridData = forecast.BuildGridDataFromWeeks(HttpContext, Request);
            return View(forecast);
        }

        [HttpGet]
        public IActionResult DailyData()
        {
            ForecastModel forecast = new ForecastModel(_inputs, _infectionModel);
            forecast.GridData = forecast.BuildGridDataFromDays(HttpContext, Request);
            return View(forecast);
        }

        [HttpGet]
        public IActionResult DailyDataExport(ForecastModel modifiedForecast)
        {
            Contract.Requires(modifiedForecast != null);

            ForecastModel forecast = new ForecastModel(_inputs, _infectionModel)
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

            ForecastModel forecast = new ForecastModel(_inputs, _infectionModel)
            {
                SelectedForecast = modifiedForecast.SelectedForecast
            };
            forecast.GridData = forecast.BuildGridDataFromDays(HttpContext, Request);
            return View(forecast);
        }

        [HttpGet]
        public IActionResult Index()
        {
            ForecastModel forecast = new ForecastModel(_inputs, _infectionModel);
            forecast.GridData = forecast.BuildGridDataFromWeeks(HttpContext, Request);
            return View(forecast);
        }

        [HttpGet]
        public IActionResult DailySurgeComparison()
        {
            ForecastModel forecast = new ForecastModel(_inputs, _infectionModel);
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