
using System;

namespace CleanTechSim.MainPage.Models
{
    public class DynamicGraphModel : BaseGraphModel
    {
        public string AjaxUri { get; }

        public decimal Median { get; }
        public decimal Dispersion { get; }
        public decimal Skew { get; }

        public DynamicGraphModel(
            string graphId,
            string title,
            string subTitle,
            string ajaxUri,
            decimal median,
            decimal dispersion,
            decimal skew)
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


