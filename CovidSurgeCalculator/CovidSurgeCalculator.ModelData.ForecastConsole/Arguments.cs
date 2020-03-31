using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace CovidSurgeCalculator.ModelData.ForecastConsole
{
    public class Arguments
    {
        [Option('r', "referenceInfectionModelBinary", Required = true, HelpText = "Path to serialized binary file containing reference infection model")]
        public string ReferenceBinaryPath { get; set; }


        [Usage(ApplicationAlias = "CovidSurgeCalculator.ModelData.ForecastConsole")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                return new List<Example>()
                {
                    new Example("Deserialize Model Binary", new Arguments() { ReferenceBinaryPath = "C:\\Output\\models.bin" })
                };
            }
        }
    }
}