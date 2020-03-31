using System;
using MessagePack;

namespace CovidSurgeCalculator.ModelData.Inputs
{
    [MessagePackObject(true)]
    public class ICUBedCount : BedCount
    {
        private decimal _icuAdmissionPercentage = 0.25m;
        public decimal ICUAdmissionPercentage
        {
            get
            {
                return _icuAdmissionPercentage;
            }
            set
            {
                if(value > 1 || value < 0)
                {
                    throw new ArgumentException("The percentage of hospitalizations that are ICU admissions must be between 0% and 100%");
                }
                _icuAdmissionPercentage = value;
            }
        }
        public int EstimatedPostICUALOS { get; set; } = 6;

        public ICUBedCount()
        {
            Beds = 1430;
            CurrentOccupancyRate = 0.75m;
            EstimatedCOVIDALOS = 10;
        }
    }
}