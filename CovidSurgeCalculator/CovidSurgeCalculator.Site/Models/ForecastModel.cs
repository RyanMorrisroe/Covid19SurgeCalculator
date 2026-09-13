using System.Collections.Generic;
using System.Diagnostics.Contracts;
using CovidSurgeCalculator.ModelData.Calculation;
using CovidSurgeCalculator.ModelData.Inputs;
using CovidSurgeCalculator.ModelData.ReferenceData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NonFactors.Mvc.Grid;
using ClosedXML.Excel;
using System.IO;

namespace CovidSurgeCalculator.Site.Models
{
    public class ForecastModel : FullForecast
    {
        public string SelectedForecast { get; set; } = "Mild Social Distancing";
        public List<string> ForecastNames { get; private set; } = new List<string>() { "Sample Mitigation", "Moderate Social Distancing", "Mild Social Distancing", "Early Pandemic" };
        public IGrid<IForecastReport> GridData { get; set; }

        public FileContentResult GridExport()
        {
            int col = 1;
            using (XLWorkbook workbook = new XLWorkbook())
            {
                IXLWorksheet sheet = workbook.Worksheets.Add("Data");

                foreach (IGridColumn column in GridData.Columns)
                {
                    sheet.Cell(1, col).Value = column.Title?.ToString();
                    sheet.Column(col).Width = 18;
                    col++;
                    column.IsEncoded = false;
                }

                foreach (IGridRow<object> row in GridData.Rows)
                {
                    col = 1;
                    foreach (IGridColumn column in GridData.Columns)
                    {
                        sheet.Cell(row.Index + 2, col).Value = column.ValueFor(row)?.ToString();
                        col++;
                    }
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return new FileContentResult(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = $"{SelectedForecast} Export.xlsx"
                    };
                }
            }
        }

        public IGrid<IForecastReport> BuildGridDataFromWeeks(HttpContext httpContext, HttpRequest request)
        {
            Contract.Requires(request != null);

            List<WeekSummary> weeks = MildForecast.WeekCalculations;
            switch(SelectedForecast)
            {
                case "Sample Mitigation":
                    weeks = MildForecast.WeekCalculations;
                    break;
                case "Moderate Social Distancing":
                    weeks = ModerateForecast.WeekCalculations;
                    break;
                case "Mild Social Distancing":
                    weeks = Aggressive80Forecast.WeekCalculations;
                    break;
                case "Early Pandemic":
                    weeks = Aggressive90Forecast.WeekCalculations;
                    break;
            }

            IGrid<IForecastReport> grid = new Grid<IForecastReport>(weeks)
            {
                HttpContext = httpContext,
                Query = request.Query
            };

            grid.Columns.Add(x => x.Number).Titled("Week Number");
            grid.Columns.Add(x => (int)x.UserOrgNonICUADC).Titled("COVID-19 Non-ICU ADC");
            grid.Columns.Add(x => (int)x.UserOrgICUADC).Titled("COVID-19 ICU ADC");
            grid.Columns.Add(x => (int)x.UserOrgVentADC).Titled("COVID-19 Ventilator ADC");
            grid.Columns.Add(x => (int)x.UserOrgNonICUBedSurplus).Titled("Non-ICU Bed Surplus/Shortage");
            grid.Columns.Add(x => (int)x.UserOrgICUBedSurplus).Titled("ICU Bed Surplus/Shortage");
            grid.Columns.Add(x => (int)x.ReferenceOrgVentSurplus).Titled("Ventilator Surplus/Shortage");

            return grid;
        }

        public IGrid<IForecastReport> BuildGridDataFromDays(HttpContext httpContext, HttpRequest request)
        {
            Contract.Requires(request != null);

            List<DaySummary> days = MildForecast.DayCalculations;
            switch (SelectedForecast)
            {
                case "Sample Mitigation":
                    days = MildForecast.DayCalculations;
                    break;
                case "Moderate Social Distancing":
                    days = ModerateForecast.DayCalculations;
                    break;
                case "Mild Social Distancing":
                    days = Aggressive80Forecast.DayCalculations;
                    break;
                case "Early Pandemic":
                    days = Aggressive90Forecast.DayCalculations;
                    break;
            }

            IGrid<IForecastReport> grid = new Grid<IForecastReport>(days)
            {
                HttpContext = httpContext,
                Query = request.Query
            };

            grid.Columns.Add(x => x.Number).Titled("Day Number");
            grid.Columns.Add(x => (int)x.UserOrgNonICUADC).Titled("COVID-19 Non-ICU ADC");
            grid.Columns.Add(x => (int)x.UserOrgICUADC).Titled("COVID-19 ICU ADC");
            grid.Columns.Add(x => (int)x.UserOrgVentADC).Titled("COVID-19 Ventilator ADC");
            grid.Columns.Add(x => (int)x.UserOrgNonICUBedSurplus).Titled("Non-ICU Bed Surplus/Shortage");
            grid.Columns.Add(x => (int)x.UserOrgICUBedSurplus).Titled("ICU Bed Surplus/Shortage");
            grid.Columns.Add(x => (int)x.ReferenceOrgVentSurplus).Titled("Ventilator Surplus/Shortage");

            return grid;
        }

        public ForecastModel()
        {

        }

        public ForecastModel(CalculatorInput inputs, ReferenceInfectionModel referenceModel) : base(inputs, referenceModel)
        {

        }
    }
}
