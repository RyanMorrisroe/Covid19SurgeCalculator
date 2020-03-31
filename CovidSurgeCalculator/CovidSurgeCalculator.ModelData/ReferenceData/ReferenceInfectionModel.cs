using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CovidSurgeCalculator.ModelData.Inputs;
using CsvHelper;
using MessagePack;

namespace CovidSurgeCalculator.ModelData.ReferenceData
{
    [MessagePackObject(true)]
    public class ReferenceInfectionModel
    {
        public Dictionary<int, DailyInfectionModel> Mild { get; internal set; } = new Dictionary<int, DailyInfectionModel>();
        public Dictionary<int, DailyInfectionModel> Moderate { get; internal set; } = new Dictionary<int, DailyInfectionModel>();
        public Dictionary<int, DailyInfectionModel> Aggressive80 { get; internal set; } = new Dictionary<int, DailyInfectionModel>();
        public Dictionary<int, DailyInfectionModel> Aggressive90 { get; internal set; } = new Dictionary<int, DailyInfectionModel>();
        public Dictionary<string, AgeCohort> AgeCohortInformation { get; internal set; } = new Dictionary<string, AgeCohort>();

        public void ApplyCalculatorInputToAgeCohortInformation(CalculatorInput inputs)
        {
            foreach(AgeCohort ageCohort in AgeCohortInformation.Values)
            {
                ageCohort.Inputs = inputs;
            }
        }

        [SerializationConstructor]
        public ReferenceInfectionModel()
        {

        }

        public ReferenceInfectionModel(Dictionary<int, DailyInfectionModel> mild,
                                       Dictionary<int, DailyInfectionModel> moderate,
                                       Dictionary<int, DailyInfectionModel> aggressive80,
                                       Dictionary<int, DailyInfectionModel> aggressive90,
                                       Dictionary<string, AgeCohort> ageCohortInformation)
        {
            Mild = mild;
            Moderate = moderate;
            Aggressive80 = aggressive80;
            Aggressive90 = aggressive90;
            AgeCohortInformation = ageCohortInformation;
        }

        public async Task WriteBinaryToDisk(string path)
        {
            using(FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                await MessagePackSerializer.SerializeAsync(stream, this, MessagePack.Resolvers.StandardResolverAllowPrivate.Options.WithCompression(MessagePackCompression.Lz4BlockArray)).ConfigureAwait(true);
            }
        }

        public static async Task<ReferenceInfectionModel> ReadBinaryFromDisk(string path, CalculatorInput inputs)
        {
            ReferenceInfectionModel model;
            using(FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                model = await MessagePackSerializer.DeserializeAsync<ReferenceInfectionModel>(stream, MessagePack.Resolvers.StandardResolverAllowPrivate.Options.WithCompression(MessagePackCompression.Lz4BlockArray)).ConfigureAwait(true);
            }
            model.ApplyCalculatorInputToAgeCohortInformation(inputs);
            return model;
        }

        public static Dictionary<int, DailyInfectionModel> ReadDailyInfectionModelFromCSV(string path)
        {
            List<DailyInfectionModel> models;
            using(StreamReader reader = new StreamReader(path))
            {
                using(CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    models = csv.GetRecords<DailyInfectionModel>().ToList();
                }
            }
            Dictionary<int, DailyInfectionModel> dict = new Dictionary<int, DailyInfectionModel>();
            foreach(DailyInfectionModel model in models)
            {
                dict.Add(model.Day, model);
            }
            return dict;
        }

        public static Dictionary<string, AgeCohort> ReadAgeCohortInformationFromCSV(string path)
        {
            List<AgeCohort> ageCohorts;
            using (StreamReader reader = new StreamReader(path))
            {
                using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Configuration.HeaderValidated = null;
                    csv.Configuration.MissingFieldFound = null;
                    ageCohorts = csv.GetRecords<AgeCohort>().ToList();
                }
            }
            Dictionary<string, AgeCohort> dict = new Dictionary<string, AgeCohort>();
            foreach (AgeCohort ageCohort in ageCohorts)
            {
                dict.Add(ageCohort.Name, ageCohort);
            }
            return dict;
        }
    }
}