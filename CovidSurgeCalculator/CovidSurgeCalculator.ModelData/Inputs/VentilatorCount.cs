using System;
using MessagePack;

namespace CovidSurgeCalculator.ModelData.Inputs
{
    [MessagePackObject(true)]
    public class VentilatorCount
    {
        public int Total { get; set; } = 1000;
        private decimal _percentageInUse = 0.70m;
        public decimal PercentageInUse
        {
            get
            {
                return _percentageInUse;
            }
            set
            {
                if(value > 1 || value < 0)
                {
                    throw new ArgumentException("Ventilator percentage in use must be between 0% and 100%");
                }
                _percentageInUse = value;
            }
        }
        public int Available
        {
            get
            {
                return (int)Math.Round(Total * (1 - PercentageInUse), MidpointRounding.ToZero);
            }
        }
        private decimal _estimatedICUPercentageRequiringVentilation = 0.66m;
        public decimal EstimatedICUPercentageRequiringVentilation
        {
            get
            {
                return _estimatedICUPercentageRequiringVentilation;
            }
            set
            {
                if(value > 1 || value < 0)
                {
                    throw new ArgumentException("Estimated ICU ventilator utilization rate must be between 0% and 100%");
                }
                _estimatedICUPercentageRequiringVentilation = value;
            }
        }
        public int EstimatedICUPatientVentilatorDays { get; set; } = 9;
    }
}