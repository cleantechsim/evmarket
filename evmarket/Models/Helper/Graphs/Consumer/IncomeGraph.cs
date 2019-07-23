
using CleanTechSim.EVMarket.Models.Helper.Graphs;

namespace CleanTechSim.EVMarket.Models.Helper.Graphs.Consumer
{

    public class IncomeGraph : InputGraph
    {
        public static readonly IncomeGraph INSTANCE = new IncomeGraph();

        private IncomeGraph()
            : base(
                    "Income",
                    "Income distribution",
                    59000m, // US median household income
                    1000m,
                    100000m,
                    10.0m,
                    3.0m,
                    DISPERSION_DEFAULT,
                    SKEW_DEFAULT)
        {

        }
    }
}