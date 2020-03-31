using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using CovidSurgeCalculator.ModelData.Inputs;
using CovidSurgeCalculator.ModelData.ReferenceData;

namespace CovidSurgeCalculator.ModelData.Calculation
{
    public class Day
    {
        #region References
        public CalculatorInput Inputs { get; set; }
        public AgeCohort AgeCohortInformation { get; set; }
        public DailyInfectionModel InfectionModel { get; set; }
        public IEnumerable<Day> Days { get; set; }
        #endregion

        #region Labels
        public int DayNumber
        {
            get
            {
                return InfectionModel.Day;
            }
        }
        public int WeekNumber
        {
            get
            {
                if(DayNumber % 7 == 0)
                {
                    return DayNumber / 7;
                }
                else
                {
                    return (DayNumber / 7) + 1;
                }           
            }
        }
        public string AgeCohortName
        {
            get
            {
                return AgeCohortInformation.Name;
            }
        }
        #endregion

        #region Population
        public int PopulationCount
        {
            get
            {
                if (Inputs.Demographics.TryGetValue(AgeCohortName, out int population))
                {
                    return population;
                }
                else
                {
                    return 0;
                }
            }
        }
        public decimal InfectedCount
        {
            get
            {
                return PopulationCount * InfectionModel.UpdatedInfectionRate * InfectionModel.OverallInfectionRate;
            }
        }
        #endregion

        public ScenarioForecast UserScenarioForecast { get; private set; } = new ScenarioForecast();
        public ScenarioForecast ReferenceScenarioForecast { get; private set; } = new ScenarioForecast();

        public decimal UserOrgNonICUADC
        {
            get
            {
                decimal sum = Days
                    .Where(x => x.DayNumber >= Math.Max(1, DayNumber - UserScenarioForecast.EstimatedCOVIDNonICUALOS + 1) && x.DayNumber <= DayNumber && x.AgeCohortName == AgeCohortName)
                    .Sum(x => x.UserScenarioForecast.OrgNonICUAdmissions);
                sum += Days
                    .Where(x => x.DayNumber >= Math.Max(1, DayNumber - UserScenarioForecast.EstimatedCOVIDICUALOS - UserScenarioForecast.EstimatedCOVIDPostICUALOS + 1) && x.DayNumber < Math.Max(1, DayNumber - UserScenarioForecast.EstimatedCOVIDICUALOS + 1) && x.AgeCohortName == AgeCohortName)
                    .Sum(x => x.UserScenarioForecast.OrgICUAdmissions);
                return sum;
            }
        }
        public decimal UserOrgICUADC
        {
            get
            {
                return Days
                    .Where(x => x.DayNumber >= Math.Max(1, DayNumber - UserScenarioForecast.EstimatedCOVIDICUALOS + 1) && x.DayNumber <= DayNumber && x.AgeCohortName == AgeCohortName)
                    .Sum(x => x.UserScenarioForecast.OrgICUAdmissions);
            }
        }
        public decimal UserOrgVentADC
        {
            get
            {
                return Days
                    .Where(x => x.DayNumber >= Math.Max(1, DayNumber - UserScenarioForecast.EstimatedCOVIDICUALOS + 1) && x.DayNumber <= DayNumber && x.AgeCohortName == AgeCohortName)
                    .Sum(x => x.UserScenarioForecast.OrgVentilatorsNeeded);
            }
        }

        public decimal ReferenceOrgNonICUADC
        {
            get
            {
                decimal sum = Days
                    .Where(x => x.DayNumber >= Math.Max(1, DayNumber - ReferenceScenarioForecast.EstimatedCOVIDNonICUALOS + 1) && x.DayNumber <= DayNumber && x.AgeCohortName == AgeCohortName)
                    .Sum(x => x.UserScenarioForecast.OrgNonICUAdmissions);
                sum += Days
                    .Where(x => x.DayNumber >= Math.Max(1, DayNumber - ReferenceScenarioForecast.EstimatedCOVIDICUALOS - ReferenceScenarioForecast.EstimatedCOVIDPostICUALOS + 1) && x.DayNumber <= Math.Max(1, DayNumber - ReferenceScenarioForecast.EstimatedCOVIDICUALOS) && x.AgeCohortName == AgeCohortName)
                    .Sum(x => x.ReferenceScenarioForecast.OrgICUAdmissions);
                return sum;
            }
        }
        public decimal ReferenceOrgICUADC
        {
            get
            {
                return Days
                    .Where(x => x.DayNumber >= Math.Max(1, DayNumber - ReferenceScenarioForecast.EstimatedCOVIDICUALOS + 1) && x.DayNumber <= DayNumber && x.AgeCohortName == AgeCohortName)
                    .Sum(x => x.ReferenceScenarioForecast.OrgICUAdmissions);
            }
        }
        public decimal ReferenceOrgVentADC
        {
            get
            {
                return Days
                    .Where(x => x.DayNumber >= Math.Max(1, DayNumber - ReferenceScenarioForecast.EstimatedCOVIDICUALOS + 1) && x.DayNumber <= DayNumber && x.AgeCohortName == AgeCohortName)
                    .Sum(x => x.ReferenceScenarioForecast.OrgVentilatorsNeeded);
            }
        }

        public Day()
        {

        }

        public Day(CalculatorInput inputs, AgeCohort ageCohortInformation, DailyInfectionModel infectionModel, List<Day> days)
        {
            Contract.Requires(inputs != null);
            Contract.Requires(ageCohortInformation != null);
            Contract.Requires(infectionModel != null);
            Contract.Requires(days != null);

            Inputs = inputs;
            AgeCohortInformation = ageCohortInformation;
            InfectionModel = infectionModel;
            Days = days;

            UserScenarioForecast = new ScenarioForecast()
            {
                DayNumber = DayNumber,
                InfectedCount = InfectedCount,
                NonICUAdmissionRate = ageCohortInformation.UserNonICUAdmissionRate,
                ICUAdmissionRate = ageCohortInformation.UserICUAdmissionRate,
                EstimatedICUPercentageRequiringVentilation = inputs.Ventilators.EstimatedICUPercentageRequiringVentilation,
                ServiceAreaHospitalCasePercentage = inputs.ServiceAreaHospitalCasePercentage,
                EstimatedCOVIDICUALOS = inputs.ICUBeds.EstimatedCOVIDALOS,
                EstimatedCOVIDPostICUALOS = inputs.ICUBeds.EstimatedPostICUALOS,
                EstimatedCOVIDNonICUALOS = inputs.NonICUBeds.EstimatedCOVIDALOS
            };
            ReferenceScenarioForecast = new ScenarioForecast()
            {
                DayNumber = DayNumber,
                InfectedCount = InfectedCount,
                NonICUAdmissionRate = ageCohortInformation.ReferenceNonICUAdmissionRate,
                ICUAdmissionRate = ageCohortInformation.ReferenceICUAdmissionRate,
                EstimatedICUPercentageRequiringVentilation = inputs.Ventilators.EstimatedICUPercentageRequiringVentilation,
                ServiceAreaHospitalCasePercentage = inputs.ServiceAreaHospitalCasePercentage,
                EstimatedCOVIDICUALOS = 10,
                EstimatedCOVIDPostICUALOS = 6,
                EstimatedCOVIDNonICUALOS = 8
            };
        }
    }
}