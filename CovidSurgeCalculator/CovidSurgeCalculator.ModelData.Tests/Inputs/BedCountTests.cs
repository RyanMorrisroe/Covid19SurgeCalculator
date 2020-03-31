using System;
using Xunit;
using CovidSurgeCalculator.ModelData.Inputs;

namespace CovidSurgeCalculator.ModelData.Tests.Inputs
{
    public class BedCountTests
    {
        private readonly BedCount bedCount = new BedCount();
        [Theory]
        [InlineData(100, .5, 50)]
        [InlineData(5, .5, 2)]
        public void AvailableBedsTest(int beds, decimal occupancy, int expected)
        {
            bedCount.Beds = beds;
            bedCount.CurrentOccupancyRate = occupancy;
            Assert.Equal(expected, bedCount.AvailableBeds);
        }

        [Theory]
        [InlineData(-0.2)]
        [InlineData(1.2)]
        public void InvalidPercentageTest(decimal occupancy)
        {
            Assert.Throws<ArgumentException>(() => { bedCount.CurrentOccupancyRate = occupancy; });
        }

        [Fact]
        public void ConstructorTest()
        {
            int beds = 50;
            decimal occupancy = 0.8m;
            int covidALOS = 4;
            BedCount newBeds = new BedCount()
            {
                Beds = beds,
                CurrentOccupancyRate = occupancy,
                EstimatedCOVIDALOS = covidALOS
            };
            Assert.Equal(beds, newBeds.Beds);
            Assert.Equal(occupancy, newBeds.CurrentOccupancyRate);
            Assert.Equal(covidALOS, newBeds.EstimatedCOVIDALOS);
        }
    }
}