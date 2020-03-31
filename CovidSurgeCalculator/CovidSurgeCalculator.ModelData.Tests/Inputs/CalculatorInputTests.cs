using System;
using Xunit;
using CovidSurgeCalculator.ModelData.Inputs;
using System.Collections.Generic;
using System.Linq;

namespace CovidSurgeCalculator.ModelData.Tests.Inputs
{
    public class CalculatorInputTests
    {
        private readonly CalculatorInput calcInput = new CalculatorInput();
        private readonly Dictionary<string, int> demographics = new Dictionary<string, int>()
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

        [Fact]
        public void TotalPopulationTest()
        {
            int total = demographics.Values.Sum();
            CalculatorInput newInput = new CalculatorInput(demographics);
            Assert.Equal(total, newInput.TotalPopulation);
        }

        [Fact]
        public void ReferenceNonICUHospitalizationTest()
        {
            calcInput.ICUBeds.ICUAdmissionPercentage = 0.4m;
            Assert.Equal(CalculatorInput.ReferenceHospitalizationRate * (1 - 0.4m), calcInput.ReferenceNonICUHospitalizationRate);
        }

        [Fact]
        public void ReferenceICUHospitalizationTest()
        {
            calcInput.ICUBeds.ICUAdmissionPercentage = 0.4m;
            Assert.Equal(CalculatorInput.ReferenceHospitalizationRate * 0.4m, calcInput.ReferenceICUHospitalizationRate);
        }

        [Theory]
        [InlineData(-0.2)]
        [InlineData(1.2)]
        public void InvalidPercentageTest(decimal percentage)
        {
            Assert.Throws<ArgumentException>(() => { calcInput.HospitalizationRate = percentage; });
            Assert.Throws<ArgumentException>(() => { calcInput.ServiceAreaHospitalCasePercentage = percentage; });
        }

        [Fact]
        public void ConstructorTest()
        {
            BedCount beds = new BedCount();
            ICUBedCount icuBeds = new ICUBedCount();
            VentilatorCount vents = new VentilatorCount();
            decimal hospitalizationRate = 0.6m;
            decimal serviceAreaHospitalizations = 0.4m;
            CalculatorInput newInput = new CalculatorInput(demographics)
            {
                HospitalizationRate = hospitalizationRate,
                ServiceAreaHospitalCasePercentage = serviceAreaHospitalizations,
                NonICUBeds = beds,
                ICUBeds = icuBeds,
                Ventilators = vents
            };
            Assert.Equal(demographics, newInput.Demographics);
            Assert.Equal(beds, newInput.NonICUBeds);
            Assert.Equal(icuBeds, newInput.ICUBeds);
            Assert.Equal(vents, newInput.Ventilators);
            Assert.Equal(hospitalizationRate, newInput.HospitalizationRate);
            Assert.Equal(serviceAreaHospitalizations, newInput.ServiceAreaHospitalCasePercentage);
        }
    }
}