using Xunit;
using CovidSurgeCalculator.ModelData.ReferenceData;

namespace CovidSurgeCalculator.ModelData.Tests.ReferenceData
{
    public class DailyInfectionModelTests
    {
        [Fact]
        public void ConstructorTest()
        {
            int day = 2;
            decimal infectionRate = 0.23231m;
            decimal updatedInfectionRate = 0.01239m;
            DailyInfectionModel dailyModel = new DailyInfectionModel()
            {
                Day = day,
                InfectionRate = infectionRate,
                UpdatedInfectionRate = updatedInfectionRate
            };
            Assert.Equal(day, dailyModel.Day);
            Assert.Equal(infectionRate, dailyModel.InfectionRate);
            Assert.Equal(updatedInfectionRate, dailyModel.UpdatedInfectionRate);
        }
    }
}