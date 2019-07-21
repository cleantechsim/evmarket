
using CleanTechSim.MainPage.Models.Helper.Graphs;

namespace CleanTechSim.MainPage.Models.Helper.Graphs.Consumer
{

    public class IncomeGraph : DynamicGraph
    {
        public static readonly IncomeGraph INSTANCE = new IncomeGraph();

        private IncomeGraph()
            : base(
                    "Income",
                    "Income distribution",
                    25000m,
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