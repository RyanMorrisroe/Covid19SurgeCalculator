using CsvHelper.Configuration.Attributes;
using MessagePack;

namespace CovidSurgeCalculator.ModelData.ReferenceData
{
    [MessagePackObject(true)]
    public class DailyInfectionModel
    {
        [Name("Day")]
        public int Day { get; set; }
        [Name("InfectionRate", "Y")]
        public decimal InfectionRate { get; set; }
        [Name("UpdatedInfectionRate", "Updated Y")]
        public decimal UpdatedInfectionRate { get; set; }
        [Name("OverallInfectionRate")]
        public decimal OverallInfectionRate { get; set; }
    }
}