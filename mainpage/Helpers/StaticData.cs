
using CleanTechSim.MainPage.Models;
using CleanTechSim.MainPage.Models.Persistent;
using CleanTechSim.MainPage.Helpers.Model;

namespace CleanTechSim.MainPage.Helpers
{
    public class StaticData
    {
        public static IMultiLineGraphModelType<MonthlyCountryEVCarSales, string> EvAdoptionGraph = new MultiLineGraphModelType<MonthlyCountryEVCarSales, string>(

            "EV percent marketshare by month",

            Encoding.YEAR_MONTH,
            Encoding.NUMBER,

            instance => instance.Year + ((decimal)instance.Month) / 100,
            instance => instance.SalesPercent,

            instance => instance.Country,
            key => key
        );

        public static ISingleLineGraphModelType<BatteryCost> BatteryCostGraph = new SingleLineGraphModelType<BatteryCost>(
            "Battery cost in $/kWh",

            Encoding.YEAR,
            Encoding.NUMBER,

            instance => instance.Century * 100 + instance.Year,
            instance => instance.Cost);

    }
}
