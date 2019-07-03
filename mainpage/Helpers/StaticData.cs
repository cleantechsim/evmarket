
using CleanTechSim.MainPage.Models;
using CleanTechSim.MainPage.Models.Persistent;
using CleanTechSim.MainPage.Helpers.Model;

namespace CleanTechSim.MainPage.Helpers
{
    public class StaticData
    {

        public static IGraphModelType<MonthlyCountryEVCarSales, string> EvAdoptionGraph = new GraphModelType<MonthlyCountryEVCarSales, string>(

            "EV percent of marketshare by month",
            Encoding.YEAR_MONTH,
            Encoding.NUMBER,

            instance => instance.Country,
            instance => instance.Year + ((decimal)instance.Month) / 100,
            instance => instance.SalesPercent,

            key => key
        );

    }
}
