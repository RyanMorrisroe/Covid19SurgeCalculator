using System.Collections.Generic;
using System.Linq;
using CovidSurgeCalculator.ModelData.Inputs;


namespace CovidSurgeCalculator.ModelData.Calculation
{
    public class WeekSummary : IForecastReport
    {
        public IEnumerable<Day> Days { get; set; }
        public CalculatorInput Inputs { get; set; }
        public int Number
        {
            get
            {
                if (Days.Any())
                {
                    return Days.First().WeekNumber;
                }
                else
                {
                    return -1;
                }
            }
        }

        public decimal InfectedCount
        {
            get
            {
                return Days.Sum(x => x.InfectedCount);
            }
        }

        public decimal UserOrgNonICUADC
        {
            get
            {
                return Days.Sum(x => x.UserOrgNonICUADC) / 7.0m;
            }
        }
        public decimal UserOrgNonICUBedSurplus
        {
            get
            {
                return Inputs.NonICUBeds.AvailableBeds - UserOrgNonICUADC;
            }
        }
        public decimal UserOrgICUADC
        {
            get
            {
                return Days.Sum(x => x.UserOrgICUADC) / 7.0m;
            }
        }
        public decimal UserOrgICUBedSurplus
        {
            get
            {
                return Inputs.ICUBeds.AvailableBeds - UserOrgICUADC;
            }
        }
        public decimal UserOrgVentADC
        {
            get
            {
                return Days.Sum(x => x.UserOrgVentADC) / 7.0m;
            }
        }
        public decimal UserOrgVentSurplus
        {
            get
            {
                return Inputs.Ventilators.Available - UserOrgICUADC;
            }
        }

        public decimal ReferenceOrgNonICUADC
        {
            get
            {
                return Days.Sum(x => x.ReferenceOrgNonICUADC) / 7.0m;
            }
        }
        public decimal ReferenceOrgNonICUBedSurplus
        {
            get
            {
                return Inputs.NonICUBeds.AvailableBeds - ReferenceOrgNonICUADC;
            }
        }
        public decimal ReferenceOrgICUADC
        {
            get
            {
                return Days.Sum(x => x.ReferenceOrgICUADC) / 7.0m;
            }
        }
        public decimal ReferenceOrgICUBedSurplus
        {
            get
            {
                return Inputs.ICUBeds.AvailableBeds - ReferenceOrgICUADC;
            }
        }
        public decimal ReferenceOrgVentADC
        {
            get
            {
                return Days.Sum(x => x.ReferenceOrgVentADC) / 7.0m;
            }
        }
        public decimal ReferenceOrgVentSurplus
        {
            get
            {
                return Inputs.Ventilators.Available - ReferenceOrgVentADC;
            }
        }
    }
}