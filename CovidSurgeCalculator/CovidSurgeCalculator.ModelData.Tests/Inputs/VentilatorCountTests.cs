using System;
using Xunit;
using CovidSurgeCalculator.ModelData.Inputs;

namespace CovidSurgeCalculator.ModelData.Tests.Inputs
{
    public class VentilatorCountTests
    {
        private readonly VentilatorCount ventCount = new VentilatorCount();

        [Theory]
        [InlineData(100, .5, 50)]
        [InlineData(5, .5, 2)]
        public void AvailableTest(int total, decimal percentInUse, int expected)
        {
            ventCount.Total = total;
            ventCount.PercentageInUse = percentInUse;
            Assert.Equal(expected, ventCount.Available);
        }

        [Theory]
        [InlineData(-0.2)]
        [InlineData(1.2)]
        public void InvalidPercentageTest(decimal percentage)
        {
            Assert.Throws<ArgumentException>(() => { ventCount.PercentageInUse = percentage; });
            Assert.Throws<ArgumentException>(() => { ventCount.EstimatedICUPercentageRequiringVentilation = percentage; });
        }

        [Fact]
        public void ConstructorTest()
        {
            int total = 30;
            decimal percentInUse = 0.1m;
            decimal percentNeedingVent = 0.05m;
            int ventDays = 6;
            VentilatorCount newVent = new VentilatorCount()
            {
                Total = total,
                PercentageInUse = percentInUse,
                EstimatedICUPercentageRequiringVentilation = percentNeedingVent,
                EstimatedICUPatientVentilatorDays = ventDays
            };
            Assert.Equal(total, newVent.Total);
            Assert.Equal(percentInUse, newVent.PercentageInUse);
            Assert.Equal(percentNeedingVent, newVent.EstimatedICUPercentageRequiringVentilation);
            Assert.Equal(ventDays, newVent.EstimatedICUPatientVentilatorDays);
        }
    }
}