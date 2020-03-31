namespace CovidSurgeCalculator.ModelData.Calculation
{
    public interface IForecastReport
    {
        int Number { get; }
        decimal InfectedCount { get; }
        decimal ReferenceOrgICUADC { get; }
        decimal ReferenceOrgICUBedSurplus { get; }
        decimal ReferenceOrgNonICUADC { get; }
        decimal ReferenceOrgNonICUBedSurplus { get; }
        decimal ReferenceOrgVentADC { get; }
        decimal ReferenceOrgVentSurplus { get; }
        decimal UserOrgICUADC { get; }
        decimal UserOrgICUBedSurplus { get; }
        decimal UserOrgNonICUADC { get; }
        decimal UserOrgNonICUBedSurplus { get; }
        decimal UserOrgVentADC { get; }
        decimal UserOrgVentSurplus { get; }
    }
}