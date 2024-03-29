
using CleanTechSim.EVMarket.Models.Domain;
using CleanTechSim.EVMarket.Models.Helper.GraphData;
using CleanTechSim.EVMarket.Models.Helper.GraphData.Prepare;

namespace CleanTechSim.EVMarket.Models.Helper.Graphs.Market
{

    internal class EVAdoptionGraph : KeyedInstanceStatsGraph<MonthlyCountryEVCarSales, string>

    {
        private static readonly IMultiLineGraphModelType<MonthlyCountryEVCarSales, string> MODEL = new MultiLineKeyedGraphModelType<MonthlyCountryEVCarSales, string>(
            "Awareness",
            "EV percent marketshare by month",

            Encoding.YEAR_MONTH,
            Encoding.NUMBER,

            instance => instance.Year + ((decimal)instance.Month) / 100,
            instance => instance.SalesPercent,

            instance => instance.Country,
            key => key
        );

        public static readonly EVAdoptionGraph INSTANCE = new EVAdoptionGraph();

        private EVAdoptionGraph()
            : base(MODEL)
        {

        }
    }
}