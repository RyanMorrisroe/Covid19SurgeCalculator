using Xunit;
using CovidSurgeCalculator.ModelData.Calculation;

namespace CovidSurgeCalculator.ModelData.Tests.Calculation
{
    public class ScenarioForecastTests
    {
        private readonly ScenarioForecast forecast;

        public ScenarioForecastTests()
        {
            forecast = new ScenarioForecast()
            {
                DayNumber = 5,
                InfectedCount = 10.0483023902462844468m,
                NonICUAdmissionRate = 0.00216m,
                ICUAdmissionRate = 0.00072m,
                EstimatedICUPercentageRequiringVentilation = 0.66m,
                ServiceAreaHospitalCasePercentage = 1.0m,
                EstimatedCOVIDICUALOS = 10,
                EstimatedCOVIDPostICUALOS = 6,
                EstimatedCOVIDNonICUALOS = 8
            };
        }

        [Fact]
        public void MarketNonICUAdmissionsTest()
        {
            Assert.Equal(0.021704333162931974405088m, forecast.MarketNonICUAdmissions);
        }

        [Fact]
        public void MarketICUAdmissionsTest()
        {
            Assert.Equal(0.007234777720977324801696m, forecast.MarketICUAdmissions);
        }

        [Fact]
        public void MarketVentilatorUsage()
        {
            Assert.Equal(0.00477495329584503436911936m, forecast.MarketVentilatorsNeeded);
        }

        [Fact]
        public void OrgNonICUAdmissionsTest()
        {
            Assert.Equal(0.0217043331629319744050880m, forecast.OrgNonICUAdmissions);
        }

        [Fact]
        public void OrgICUAdmissionsTest()
        {
            Assert.Equal(0.0072347777209773248016960m, forecast.OrgICUAdmissions);
        }

        [Fact]
        public void OrgVentilatorUsage()
        {
            Assert.Equal(0.004774953295845034369119360m, forecast.OrgVentilatorsNeeded);
        }
    }
}