
using CleanTechSim.MainPage.Models.Domain;
using CleanTechSim.MainPage.Models.Helper.GraphData;
using CleanTechSim.MainPage.Models.Helper.GraphData.Prepare;

namespace CleanTechSim.MainPage.Models.Helper.Graphs.Market
{

    public class EVAdoptionGraph
    {
        public static readonly IMultiLineGraphModelType<MonthlyCountryEVCarSales, string> MODEL = new MultiLineKeyedGraphModelType<MonthlyCountryEVCarSales, string>(
            "Awareness",
            "EV percent marketshare by month",

            Encoding.YEAR_MONTH,
            Encoding.NUMBER,

            instance => instance.Year + ((decimal)instance.Month) / 100,
            instance => instance.SalesPercent,

            instance => instance.Country,
            key => key
        );
    }

}