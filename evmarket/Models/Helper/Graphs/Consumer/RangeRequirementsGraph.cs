
namespace CleanTechSim.EVMarket.Models.Helper.Graphs.Consumer
{
    public class RangeRequirementsGraph : InputGraph
    {
        public static readonly RangeRequirementsGraph INSTANCE = new RangeRequirementsGraph();

        private RangeRequirementsGraph() : base(
                    "Range",
                    "Requirement for range",
                    250m,
                    50m,
                    1000m,
                    15.0m,
                    3.0m,
                    DISPERSION_DEFAULT,
                    SKEW_DEFAULT
        )
        {

        }
    }
}


