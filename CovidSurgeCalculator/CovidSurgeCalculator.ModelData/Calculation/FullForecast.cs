using System.Diagnostics.Contracts;
using CovidSurgeCalculator.ModelData.Inputs;
using CovidSurgeCalculator.ModelData.ReferenceData;

namespace CovidSurgeCalculator.ModelData.Calculation
{
    public class FullForecast
    {
        public CalculatorInput Inputs { get; set; }
        public ReferenceInfectionModel ReferenceModel { get; set; }

        public Forecast MildForecast { get; set; }
        public Forecast ModerateForecast { get; set; }
        public Forecast Aggressive80Forecast { get; set; }
        public Forecast Aggressive90Forecast { get; set; }

        public FullForecast()
        {

        }

        public FullForecast(CalculatorInput inputs, ReferenceInfectionModel referenceModel)
        {
            Contract.Requires(inputs != null);
            Contract.Requires(referenceModel != null);

            Inputs = inputs;
            ReferenceModel = referenceModel;

            MildForecast = new Forecast(inputs, referenceModel.AgeCohortInformation.Values, referenceModel.Mild.Values);
            ModerateForecast = new Forecast(inputs, referenceModel.AgeCohortInformation.Values, referenceModel.Moderate.Values);
            Aggressive80Forecast = new Forecast(inputs, referenceModel.AgeCohortInformation.Values, referenceModel.Aggressive80.Values);
            Aggressive90Forecast = new Forecast(inputs, referenceModel.AgeCohortInformation.Values, referenceModel.Aggressive90.Values);
        }
    }
}