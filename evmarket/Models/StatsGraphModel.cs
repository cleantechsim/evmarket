

using System;

using CleanTechSim.EVMarket.Models.Helper.ClientGraph;

namespace CleanTechSim.EVMarket.Models
{
    public class StatsGraphModel : BaseGraphModel
    {
        public PreparedDataPoints DataPoints { get; }

        public StatsGraphModel(string graphId, string title, string subTitle, PreparedDataPoints dataPoints)
            : base(graphId, title, subTitle)
        {
            if (dataPoints == null)
            {
                throw new ArgumentNullException();
            }

            this.DataPoints = dataPoints;
        }
    }
}