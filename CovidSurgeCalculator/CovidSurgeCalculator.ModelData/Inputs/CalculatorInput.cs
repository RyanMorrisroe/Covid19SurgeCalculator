using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MessagePack;

namespace CovidSurgeCalculator.ModelData.Inputs
{
    [MessagePackObject(true)]
    public class CalculatorInput
    {
        public Dictionary<string, int> Demographics { get; private set; } = new Dictionary<string, int>();
        public int TotalPopulation
        {
            get
            {
                return Demographics.Sum(x => x.Value);
            }
        }
        private decimal _hospitalizationRate = 0.012m;
        public decimal HospitalizationRate
        {
            get
            {
                return _hospitalizationRate;
            }
            set
            {
                if (value > 1 || value < 0)
                {
                    throw new ArgumentException("Hospitalization rate must be between 0% and 100%");
                }
                _hospitalizationRate = value;
            }
        }
        private decimal _serviceAreaHospitalCasePercentage = 1.00m;
        public decimal ServiceAreaHospitalCasePercentage
        {
            get
            {
                return _serviceAreaHospitalCasePercentage;
            }
            set
            {
                if(value > 1 || value < 0)
                {
                    throw new ArgumentException("Service Area Cases for your hospital must be between 0% and 100%");
                }
                _serviceAreaHospitalCasePercentage = value;
            }
        }
        public BedCount NonICUBeds { get; set; } = new BedCount();
        public ICUBedCount ICUBeds { get; set; } = new ICUBedCount();
        public VentilatorCount Ventilators { get; set; } = new VentilatorCount();

        public const decimal ReferenceHospitalizationRate = 0.05m;
        public decimal ReferenceNonICUHospitalizationRate
        {
            get
            {
                return ReferenceHospitalizationRate * (1 - ICUBeds.ICUAdmissionPercentage);
            }
        }
        public decimal ReferenceICUHospitalizationRate
        {
            get
            {
                return ReferenceHospitalizationRate * ICUBeds.ICUAdmissionPercentage;
            }
        }

        public CalculatorInput()
        {

        }

        public CalculatorInput(Dictionary<string, int> demographics)
        {
            Demographics = demographics;
        }

        public async Task WriteBinaryToDisk(string path)
        {
            using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                await MessagePackSerializer.SerializeAsync(stream, this, MessagePack.Resolvers.StandardResolverAllowPrivate.Options.WithCompression(MessagePackCompression.Lz4BlockArray)).ConfigureAwait(true);
            }
        }

        public static async Task<CalculatorInput> ReadBinaryFromDisk(string path)
        {
            CalculatorInput inputs;
            using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                inputs = await MessagePackSerializer.DeserializeAsync<CalculatorInput>(stream, MessagePack.Resolvers.StandardResolverAllowPrivate.Options.WithCompression(MessagePackCompression.Lz4BlockArray)).ConfigureAwait(true);
            }
            return inputs;
        }
    }
}