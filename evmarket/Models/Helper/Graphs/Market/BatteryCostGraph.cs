
using CleanTechSim.EVMarket.Models.Domain;
using CleanTechSim.EVMarket.Models.Helper.GraphData;
using CleanTechSim.EVMarket.Models.Helper.GraphData.Prepare;

namespace CleanTechSim.EVMarket.Models.Helper.Graphs.Market
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
