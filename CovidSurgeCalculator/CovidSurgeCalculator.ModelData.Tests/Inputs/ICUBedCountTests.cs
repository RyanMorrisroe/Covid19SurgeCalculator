using System;
using Xunit;
using CovidSurgeCalculator.ModelData.Inputs;

namespace CovidSurgeCalculator.ModelData.Tests.Inputs
{
    public class ICUBedCountTests
    {
        private readonly ICUBedCount icuBeds = new ICUBedCount();
        
        [Fact]
        public void ConstructorTest()
        {
            int beds = 50;
            decimal occupancy = 0.8m;
            decimal icuAdmitPercentage = 0.1m;
            int covidALOS = 4;
            int icuPostALOS = 5;
            ICUBedCount newBeds = new ICUBedCount()
            {
                Beds = beds,
                CurrentOccupancyRate = occupancy,
                EstimatedCOVIDALOS = covidALOS,
                ICUAdmissionPercentage = icuAdmitPercentage,
                EstimatedPostICUALOS = icuPostALOS
            };
            Assert.Equal(beds, newBeds.Beds);
            Assert.Equal(occupancy, newBeds.CurrentOccupancyRate);
            Assert.Equal(covidALOS, newBeds.EstimatedCOVIDALOS);
            Assert.Equal(icuAdmitPercentage, newBeds.ICUAdmissionPercentage);
            Assert.Equal(icuPostALOS, newBeds.EstimatedPostICUALOS);
        }

        [Theory]
        [InlineData(-0.2)]
        [InlineData(1.2)]
        public void InvalidPercentageTest(decimal admitPercentage)
        {
            Assert.Throws<ArgumentException>(() => { icuBeds.ICUAdmissionPercentage = admitPercentage; });
        }
    }
}