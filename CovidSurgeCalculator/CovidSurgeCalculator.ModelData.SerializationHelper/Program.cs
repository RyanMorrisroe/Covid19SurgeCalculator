using System.Collections.Generic;
using System.Linq;
using CommandLine;
using CovidSurgeCalculator.ModelData.Inputs;
using CovidSurgeCalculator.ModelData.ReferenceData;

namespace CovidSurgeCalculator.ModelData.SerializationHelper
{
    class Program
    {
        static int Main(string[] args)
        {
            int result = Parser.Default.ParseArguments<Arguments>(args).MapResult(args => Serialize(args), errs => HandleParseErrors(errs));
            return result;
        }

        static int Serialize(Arguments args)
        {
            Dictionary<string, AgeCohort> ageCohortInformation = ReferenceInfectionModel.ReadAgeCohortInformationFromCSV(args.AgeCohortInformationCsvPath);
            Dictionary<int, DailyInfectionModel> mild = ReferenceInfectionModel.ReadDailyInfectionModelFromCSV(args.MildCsvPath);
            Dictionary<int, DailyInfectionModel> moderate = ReferenceInfectionModel.ReadDailyInfectionModelFromCSV(args.ModerateCsvPath);
            Dictionary<int, DailyInfectionModel> aggressive80 = ReferenceInfectionModel.ReadDailyInfectionModelFromCSV(args.Aggressive80CsvPath);
            Dictionary<int, DailyInfectionModel> aggressive90 = ReferenceInfectionModel.ReadDailyInfectionModelFromCSV(args.Aggressive90CsvPath);
            ReferenceInfectionModel model = new ReferenceInfectionModel(mild, moderate, aggressive80, aggressive90, ageCohortInformation);
            model.WriteBinaryToDisk(args.ReferenceLookupOutputFilePath).Wait();

            Dictionary<string, int> demographics = new Dictionary<string, int>()
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
            CalculatorInput inputs = new CalculatorInput(demographics);
            inputs.WriteBinaryToDisk(args.CalculatorInputFilePath).Wait();
            inputs = CalculatorInput.ReadBinaryFromDisk(args.CalculatorInputFilePath).Result; //read back in to make sure serialization was truly successful
            ReferenceInfectionModel _ = ReferenceInfectionModel.ReadBinaryFromDisk(args.ReferenceLookupOutputFilePath, inputs).Result; //read back in to make sure that serialization was truly successful
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
