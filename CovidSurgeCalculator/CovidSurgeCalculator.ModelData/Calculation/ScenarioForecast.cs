using System;

namespace CovidSurgeCalculator.ModelData.Calculation
{
    public class ScenarioForecast
    {
        public int DayNumber { get; set; }
        public decimal InfectedCount { get; set; }
        public decimal NonICUAdmissionRate { get; set; }
        public decimal ICUAdmissionRate { get; set; }
        public decimal EstimatedICUPercentageRequiringVentilation { get; set; }
        public decimal ServiceAreaHospitalCasePercentage { get; set; }
        public int EstimatedCOVIDICUALOS { get; set; }
        public int EstimatedCOVIDPostICUALOS { get; set; }
        public int EstimatedCOVIDNonICUALOS { get; set; }

        public ScenarioForecast()
        {

        }

        #region Market
        public decimal MarketNonICUAdmissions
        {
            get
            {
                return InfectedCount * NonICUAdmissionRate;
            }
        }
        public decimal MarketICUAdmissions
        {
            get
            {
                return InfectedCount * ICUAdmissionRate;
            }
        }
        public decimal MarketVentilatorsNeeded
        {
            get
            {
                return MarketICUAdmissions * EstimatedICUPercentageRequiringVentilation;
            }
        }
        #endregion

        #region Org
        public decimal OrgNonICUAdmissions
        {
            get
            {
                return MarketNonICUAdmissions * ServiceAreaHospitalCasePercentage;
            }
        }
        public decimal OrgICUAdmissions
        {
            get
            {
                return MarketICUAdmissions * ServiceAreaHospitalCasePercentage;
            }
        }
        public decimal OrgVentilatorsNeeded
        {
            get
            {
                return MarketVentilatorsNeeded * ServiceAreaHospitalCasePercentage;
            }
        }
        #endregion
    }
}