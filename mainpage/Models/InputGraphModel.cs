
using System;
using CleanTechSim.MainPage.Models.Helper.GraphData.Prepare;

namespace CleanTechSim.MainPage.Models
{
    public class Range
    {
        public decimal Min { get; }
        public decimal Initial { get; }
        public decimal Max { get; }
        public decimal Step { get; }

        public Range(decimal min, decimal initial, decimal max, decimal step)
        {
            this.Min = min;
            this.Initial = initial;
            this.Max = max;
            this.Step = step;
        }
    }

    public class InputGraphModel : BaseGraphModel
    {
        public string AjaxUri { get; }

        public decimal Median { get; }
        public Range Dispersion { get; }
        public Range Skew { get; }

        public InputGraphModel(
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


