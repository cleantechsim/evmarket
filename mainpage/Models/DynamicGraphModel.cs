
using System;
using CleanTechSim.MainPage.Models.Helper.GraphData.Prepare;

namespace CleanTechSim.MainPage.Models
{
    public class DynamicGraphModel : BaseGraphModel
    {
        public string AjaxUri { get; }

        public decimal Median { get; }
        public Range Dispersion { get; }
        public Range Skew { get; }

        public DynamicGraphModel(
            string graphId,
            string title,
            string subTitle,
            string ajaxUri,
            decimal median,
            Range dispersion,
            Range skew)
            : base(graphId, title, subTitle)
        {
            if (ajaxUri == null)
            {
                throw new ArgumentNullException();
            }

            this.AjaxUri = ajaxUri;
            this.Median = median;
            this.Dispersion = dispersion;
            this.Skew = skew;
        }
    }
}


