
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

        // For storing in JS window object so can add eventhandlers and such
        public string DOMWindowVariableName()
        {
            return "_varName" + GraphId;
        }
    }

    public class AjaxGraphModel : BaseGraphModel
    {

        public string AjaxUrl { get; }

        internal AjaxGraphModel(string graphId, string title, string subTitle, string ajaxUrl)
            : base(graphId, title, subTitle)
        {
            this.AjaxUrl = ajaxUrl;
        }
    }
}