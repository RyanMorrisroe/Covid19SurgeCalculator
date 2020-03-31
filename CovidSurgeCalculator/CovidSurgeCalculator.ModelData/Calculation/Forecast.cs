using System.Collections.Generic;
using System.Linq;
using CovidSurgeCalculator.ModelData.Inputs;
using CovidSurgeCalculator.ModelData.ReferenceData;

namespace CovidSurgeCalculator.ModelData.Calculation
{
    public class Forecast
    {
        public List<Day> Days { get; private set; }
        public List<DaySummary> DayCalculations { get; private set; }
        public List<WeekSummary> WeekCalculations { get; private set; }

        public Forecast(CalculatorInput inputs, IEnumerable<AgeCohort> ageCohortInformation, IEnumerable<DailyInfectionModel> dailyInfectionModels)
        {
            Days = GenerateDays(inputs, ageCohortInformation, dailyInfectionModels);
            DayCalculations = GenerateDayCalculations(inputs, Days);
            WeekCalculations = GenerateWeekCalculations(inputs, Days);
        }

        public static List<Day> GenerateDays(CalculatorInput inputs, IEnumerable<AgeCohort> ageCohortInformation, IEnumerable<DailyInfectionModel> dailyInfectionModels)
        {
            List<Day> days = new List<Day>();
            foreach(DailyInfectionModel infectionModel in dailyInfectionModels.ToList())
            {
                foreach (AgeCohort ageCohort in ageCohortInformation.ToList())
                {
                    Day day = new Day(inputs, ageCohort, infectionModel, days);
                    days.Add(day);
                }            
            }
            return days;
        }

        public static List<DaySummary> GenerateDayCalculations(CalculatorInput inputs, List<Day> days)
        {
            List<DaySummary> daySummaries = new List<DaySummary>();
            List<int> dayNumbers = days.Select(x => x.DayNumber).Distinct().OrderBy(x => x).ToList();
            foreach(int dayNumber in dayNumbers)
            {
                DaySummary day = new DaySummary()
                {
                    Days = days.Where(x => x.DayNumber == dayNumber),
                    Inputs = inputs
                };
                daySummaries.Add(day);
            }
            return daySummaries;
        }

        public static List<WeekSummary> GenerateWeekCalculations(CalculatorInput inputs, List<Day> days)
        {
            List<WeekSummary> weeks = new List<WeekSummary>();
            List<int> weekNumbers = days.Select(x => x.WeekNumber).Distinct().OrderBy(x => x).ToList();
            foreach(int weekNumber in weekNumbers)
            {
                WeekSummary week = new WeekSummary()
                {
                    Days = days.Where(x => x.WeekNumber == weekNumber),
                    Inputs = inputs
                };
                weeks.Add(week);
            }
            return weeks;
        }
    }
}