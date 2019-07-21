
using CleanTechSim.MainPage.Models.Helper.Graphs;

namespace CleanTechSim.MainPage.Models.Helper.Graphs
{
    public class EVPurchasePropensityGraph : InputGraph
    {
        public static readonly EVPurchasePropensityGraph INSTANCE = new EVPurchasePropensityGraph();

        private EVPurchasePropensityGraph()

            : base(
                    "Propensity for EV purchase",
                    "All else being equal",
                    50m,
                    0m,
                    100m,
                    15.0m,
                    3.0m,
                    DISPERSION_DEFAULT,
                    SKEW_DEFAULT,

                    100m) // suggested max x, used by graph generator to limit values

        {

        }
    }
}
