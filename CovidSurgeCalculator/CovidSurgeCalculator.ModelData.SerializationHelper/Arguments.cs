using System.Collections.Generic;
using System.IO;
using CommandLine;
using CommandLine.Text;

namespace CovidSurgeCalculator.ModelData.SerializationHelper
{
    [Verb("serialize", HelpText = "Reads model data from CSVs and serializes into the MapPack binary format")]
    public class Arguments
    {
        [Option('c', "csvfolderpath", Required = true, HelpText = "Path to folder containing csv files to load")]
        public string CsvFolderPath { get; set; }
        public string MildCsvPath
        {
            get
            {
                return Path.Combine(CsvFolderPath, "Mild.csv");
            }
        }
        public string ModerateCsvPath
        {
            get
            {
                return Path.Combine(CsvFolderPath, "Moderate.csv");
            }
        }
        public string Aggressive80CsvPath
        {
            get
            {
                return Path.Combine(CsvFolderPath, "Aggressive80.csv");
            }
        }
        public string Aggressive90CsvPath
        {
            get
            {
                return Path.Combine(CsvFolderPath, "Aggressive90.csv");
            }
        }
        public string AgeCohortInformationCsvPath
        {
            get
            {
                return Path.Combine(CsvFolderPath, "AgeCohortInformation.csv");
            }
        }

        [Option('r', "referencelookupoutputfilepath", Required = true, HelpText = "Path to write the reference lookup MapPack binary to")]
        public string ReferenceLookupOutputFilePath { get; set; }

        [Option('i', "calculatorinputfilepath", Required = true, HelpText = "Path to write the calculator input MapPack binary to")]
        public string CalculatorInputFilePath { get; set; }

        [Usage(ApplicationAlias = "CovidSurgeCalculator.ModelData.SerializationHelper")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                return new List<Example>()
                {
                    new Example("Serialize Model CSVs", new Arguments() { CsvFolderPath = "C:\\CSVs\\", ReferenceLookupOutputFilePath = "C:\\Output\\models.bin", CalculatorInputFilePath = "C:\\Output\\inputs.bin" })
                };
            }
        }
    }
}