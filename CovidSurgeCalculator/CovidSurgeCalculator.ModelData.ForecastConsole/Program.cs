using System.Collections.Generic;
using System.Linq;
using CommandLine;
using CovidSurgeCalculator.ModelData.Calculation;
using CovidSurgeCalculator.ModelData.Inputs;
using CovidSurgeCalculator.ModelData.ReferenceData;

namespace CovidSurgeCalculator.ModelData.ForecastConsole
{
    class Program
    {
        private static readonly Dictionary<string, int> demographics = new Dictionary<string, int>()
                                                                    {
                                                                        {"0-9", 152599 },
                                                                        {"10-19", 156335 },
                                                                        {"20-29", 257784 },
                                                                        {"30-39", 308683 },
                                                                        {"40-49", 230953 },
                                                                        {"50-59", 204307 },
                                                                        {"60-69", 176602 },
                                                                        {"70-79", 110792 },
                                                                        {"80+", 67466 }
                                                                    };

        static int Main(string[] args)
        {
            int result = Parser.Default.ParseArguments<Arguments>(args).MapResult(args => RunProgram(args), errs => HandleParseErrors(errs));
            return result;
        }

        static int RunProgram(Arguments args)
        {
            CalculatorInput inputs = new CalculatorInput(demographics);
            ReferenceInfectionModel referenceModel = ReferenceInfectionModel.ReadBinaryFromDisk(args.ReferenceBinaryPath, inputs).Result;
            FullForecast forecast = new FullForecast(inputs, referenceModel);
            return 0;
        }

        static int HandleParseErrors(IEnumerable<Error> errs)
        {
            int result = -2;
            if (errs.Any(x => x is HelpRequestedError || x is VersionRequestedError))
            {
                result = -1;
            }
            return result;
        }
    }
}
