using System;
using CsvHelper.Configuration.Attributes;
using MessagePack;
using CovidSurgeCalculator.ModelData.Inputs;

namespace CovidSurgeCalculator.ModelData.ReferenceData
{
    [MessagePackObject(true)]
    public class AgeCohort
    {
        [Name("AgeCohort", "Name")]
        public string Name { get; set; }
        [Ignore]
        [IgnoreMember]
        public CalculatorInput Inputs { get; set; }

        private decimal _percentSymptomaticCasesRequringHospitalization;
        [Name("PercentSymptomaticCasesRequringHospitalization")]
        public decimal PercentSymptomaticCasesRequringHospitalization
        {
            get
            {
                return _percentSymptomaticCasesRequringHospitalization;
            }
            set
            {
                if(value > 1 || value < 0)
                {
                    throw new ArgumentException("Percentage must be between 0% and 100%");
                }
                _percentSymptomaticCasesRequringHospitalization = value;
            }
        }
        [Ignore]
        [IgnoreMember]
        public decimal HospitalizationMultiplier
        {
            get
            {
                return PercentSymptomaticCasesRequringHospitalization / CalculatorInput.ReferenceHospitalizationRate;
            }
        }


        private decimal _percentHospitalizedCasesRequringCriticalCare;
        [Name("PercentHospitalizedCasesRequringCriticalCare")]
        public decimal PercentHospitalizedCasesRequringCriticalCare
        {
            get
            {
                return _percentHospitalizedCasesRequringCriticalCare;
            }
            set
            {
                if(value > 1 || value < 0)
                {
                    throw new ArgumentException("Percentage must be between 0% and 100%");
                }
                _percentHospitalizedCasesRequringCriticalCare = value;
            }
        }
        [Ignore]
        [IgnoreMember]
        public decimal CriticalCareMultiplier
        {
            get
            {
                return _percentHospitalizedCasesRequringCriticalCare / Inputs.ICUBeds.ICUAdmissionPercentage;
            }
        }

        private decimal _infectionFatalityRatio;
        [Name("InfectionFatalityRatio")]
        public decimal InfectionFatalityRatio
        {
            get
            {
                return _infectionFatalityRatio;
            }
            set
            {
                if (value > 1 || value < 0)
                {
                    throw new ArgumentException("Percentage must be between 0% and 100%");
                }
                _infectionFatalityRatio = value;
            }
        }

        [Ignore]
        [IgnoreMember]
        public decimal ReferenceTotalAdmissionRate
        {
            get
            {
                return HospitalizationMultiplier * CalculatorInput.ReferenceHospitalizationRate;
            }
        }
        [Ignore]
        [IgnoreMember]
        public decimal UserTotalAdmissionRate
        {
            get
            {
                return HospitalizationMultiplier * Inputs.HospitalizationRate;
            }
        }

        [Ignore]
        [IgnoreMember]
        public decimal ReferenceNonICUAdmissionRate
        {
            get
            {
                return ReferenceTotalAdmissionRate * (1 - Inputs.ICUBeds.ICUAdmissionPercentage);
            }
        }
        [Ignore]
        [IgnoreMember]
        public decimal UserNonICUAdmissionRate
        {
            get
            {
                return UserTotalAdmissionRate * (1 - Inputs.ICUBeds.ICUAdmissionPercentage);
            }
        }

        [Ignore]
        [IgnoreMember]
        public decimal ReferenceICUAdmissionRate
        {
            get
            {
                return ReferenceTotalAdmissionRate * Inputs.ICUBeds.ICUAdmissionPercentage;
            }
        }
        [Ignore]
        [IgnoreMember]
        public decimal UserICUAdmissionRate
        {
            get
            {
                return UserTotalAdmissionRate * Inputs.ICUBeds.ICUAdmissionPercentage;
            }
        }
    }
}