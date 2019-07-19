

using System;

using CleanTechSim.MainPage.Models.Helper.ClientGraph;

namespace CleanTechSim.MainPage.Models
{
    public class StaticGraphModel : BaseGraphModel
    {
        public PreparedDataPoints DataPoints { get; }

        public StaticGraphModel(string graphId, string title, string subTitle, PreparedDataPoints dataPoints)
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