
using CleanTechSim.MainPage.Models.Domain;
using CleanTechSim.MainPage.Models.Helper.GraphData;
using CleanTechSim.MainPage.Models.Helper.GraphData.Prepare;

namespace CleanTechSim.MainPage.Models.Helper.Graphs.Market
{
    internal class BatteryCostGraph : InstanceStatsGraph<BatteryCost, object>
    {
        private static readonly ISingleLineGraphModelType<BatteryCost> MODEL = new SingleLineGraphModelType<BatteryCost>(
            "Cost",
            "Battery cost in $/kWh",

            Encoding.YEAR,
            Encoding.NUMBER,

            instance => instance.Century * 100 + instance.Year,
            instance => instance.Cost
        );

        internal static readonly BatteryCostGraph INSTANCE = new BatteryCostGraph();

        private BatteryCostGraph()
            : base(MODEL)
        {

        }

    }

}
