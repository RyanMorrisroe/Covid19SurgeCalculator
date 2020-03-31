using System;
using MessagePack;

namespace CovidSurgeCalculator.ModelData.Inputs
{
    [MessagePackObject(true)]
    public class BedCount
    {
        public int Beds { get; set; } = 14800;
        private decimal _currentOccupancyRate = 0.80m;
        public decimal CurrentOccupancyRate
        {
            get
            {
                return _currentOccupancyRate;
            }
            set
            {
                if(value > 1 || value < 0)
                {
                    throw new ArgumentException("Current occupancy rate must be between 0% and 100%");
                }
                _currentOccupancyRate = value;
            }
        }
        public int AvailableBeds
        {
            get
            {
                return (int)Math.Round(Beds * (1 - CurrentOccupancyRate), MidpointRounding.ToZero);
            }
        }
        public int EstimatedCOVIDALOS { get; set; } = 8;
    }
}