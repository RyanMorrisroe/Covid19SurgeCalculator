using System;
using Xunit;
using CovidSurgeCalculator.ModelData.Inputs;
using CovidSurgeCalculator.ModelData.ReferenceData;

namespace CovidSurgeCalculator.ModelData.Tests.ReferenceData
{
    public class DemographicAdmissionRateTests
    {
        private readonly AgeCohort ageCohort = new AgeCohort();
        private readonly CalculatorInput inputs = new CalculatorInput();

        public DemographicAdmissionRateTests()
        {
            ageCohort.Name = "70-79";
            ageCohort.PercentSymptomaticCasesRequringHospitalization = 0.243m;
            ageCohort.PercentHospitalizedCasesRequringCriticalCare = 0.432m;
            ageCohort.InfectionFatalityRatio = 0.0510m;
            ageCohort.Inputs = inputs;
        }

        [Fact]
        public void HospitalizationMultiplierTest()
        {     
            Assert.Equal(4.86m, ageCohort.HospitalizationMultiplier);
        }

        [Fact]
        public void CriticalCareMultiplierTest()
        {
            Assert.Equal(1.728m, ageCohort.CriticalCareMultiplier);
        }

        [Fact]
        public void ReferenceTotalAdmissionRateTest()
        {
            Assert.Equal(0.243m, ageCohort.ReferenceTotalAdmissionRate);
        }

        [Fact]
        public void UserTotalAdmissionRateTest()
        {
            Assert.Equal(0.05832m, ageCohort.UserTotalAdmissionRate);
        }

        [Fact]
        public void ReferenceNonICUAdmissionRateTest()
        {
            Assert.Equal(0.18225m, ageCohort.ReferenceNonICUAdmissionRate);
        }

        [Fact]
        public void UserNonICUAdmissionRateTest()
        {
            Assert.Equal(0.04374m, ageCohort.UserNonICUAdmissionRate);
        }

        [Fact]
        public void ReferenceICUAdmissionRateTest()
        {
            Assert.Equal(0.06075m, ageCohort.ReferenceICUAdmissionRate);
        }

        [Fact]
        public void UserICUAdmissionRateTest()
        {
            Assert.Equal(0.01458m, ageCohort.UserICUAdmissionRate);
        }

        [Theory]
        [InlineData(-0.2)]
        [InlineData(1.2)]
        public void InvalidPercentageTest(decimal percentage)
        {
            Assert.Throws<ArgumentException>(() => { ageCohort.PercentSymptomaticCasesRequringHospitalization = percentage; });
            Assert.Throws<ArgumentException>(() => { ageCohort.PercentHospitalizedCasesRequringCriticalCare = percentage; });
            Assert.Throws<ArgumentException>(() => { ageCohort.InfectionFatalityRatio = percentage; });
        }

        [Fact]
        public void ConstructorTest()
        {
            string name = "00-10";
            decimal hospitalizationRate = 0.3m;
            decimal icuRate = 0.4m;
            decimal fatalityRatio = 0.09m;
            AgeCohort newCohort = new AgeCohort()
            {
                Name = name,
                PercentSymptomaticCasesRequringHospitalization = hospitalizationRate,
                PercentHospitalizedCasesRequringCriticalCare = icuRate,
                InfectionFatalityRatio = fatalityRatio,
                Inputs = inputs
            };
            Assert.Equal(inputs, newCohort.Inputs);
            Assert.Equal(name, newCohort.Name);
            Assert.Equal(hospitalizationRate, newCohort.PercentSymptomaticCasesRequringHospitalization);
            Assert.Equal(icuRate, newCohort.PercentHospitalizedCasesRequringCriticalCare);
            Assert.Equal(fatalityRatio, newCohort.InfectionFatalityRatio);
        }
    }
}