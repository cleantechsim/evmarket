
using System;

namespace CleanTechSim.MainPage.Models
{
    public abstract class BaseGraphModel
    {
        public string GraphId { get; }
        public string Title { get; }
        public string SubTitle { get; }

        internal BaseGraphModel(string graphId, string title, string subTitle)
        {
            if (graphId == null)
            {
                throw new ArgumentNullException();
            }

            this.GraphId = graphId;
            this.Title = title;
            this.SubTitle = subTitle;
        }
    }
}