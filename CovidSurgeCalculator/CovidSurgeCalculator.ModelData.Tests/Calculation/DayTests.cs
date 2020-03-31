using System.Collections.Generic;
using Xunit;
using CovidSurgeCalculator.ModelData.Calculation;
using CovidSurgeCalculator.ModelData.Inputs;
using CovidSurgeCalculator.ModelData.ReferenceData;

namespace CovidSurgeCalculator.ModelData.Tests.Calculation
{
    public class DayTests
    {
        private readonly Day day = new Day();
        private readonly Dictionary<string, int> demographics;
        private readonly CalculatorInput input;
        private readonly AgeCohort ageCohort;
        private readonly DailyInfectionModel infectionModel;

        public DayTests()
        {      
            demographics = new Dictionary<string, int>()
                                                {
                                                    {"0-9", 152599 },
                                                    {"10-19", 156335 },
                                                    {"20-29", 257784 },
                                                    {"30-39", 308683 },
                                                    {"40-49", 230953 },
                                                    {"50-59", 204307 },
                                                    {"60-69", 176602 },
                                                    {"70-79", 110792 },
                                                    {"80+", 67466 }
                                                };
            input = new CalculatorInput(demographics);
            ageCohort = new AgeCohort()
            {
                Name = "20-29",
                Inputs = input,
                PercentSymptomaticCasesRequringHospitalization = 0.25m,
                PercentHospitalizedCasesRequringCriticalCare = 0.10m,
                InfectionFatalityRatio = 0.05m
            };
            infectionModel = new DailyInfectionModel()
            {
                Day = 5,
                InfectionRate = 0.00000458005153804181m,
                UpdatedInfectionRate = 0.00004379723879205500m,
                OverallInfectionRate = 0.89m
            };
            day.Inputs = input;
            day.AgeCohortInformation = ageCohort;
            day.InfectionModel = infectionModel;
        }

        [Fact]
        public void DayNumberTest()
        {
            Assert.Equal(infectionModel.Day, day.DayNumber);
        }

        [Theory]
        [InlineData(5, 1)]
        [InlineData(7, 1)]
        [InlineData(15, 3)]
        [InlineData(37, 6)]
        public void WeekNumberTest(int dayNumber, int expected)
        {
            day.InfectionModel.Day = dayNumber;
            Assert.Equal(expected, day.WeekNumber);
        }

        [Fact]
        public void AgeCohortNameTest()
        {
            Assert.Equal(ageCohort.Name, day.AgeCohortName);
        }

        [Fact]
        public void PopulationCountTest()
        {
            Assert.Equal(input.Demographics[ageCohort.Name], day.PopulationCount);
        }

        [Fact]
        public void PopulationCountNoDemographicsTest()
        {
            day.AgeCohortInformation.Name = "fake";
            Assert.Equal(0, day.PopulationCount);
        }

        [Fact]
        public void InfectedCountTest()
        {
            Assert.Equal(10.0483023902462844468m, day.InfectedCount);
        }
    }
}