
using System;
using CleanTechSim.EVMarket.Models.Helper.GraphData.Prepare;

namespace CleanTechSim.EVMarket.Models
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

    public class InputGraphModel : AjaxGraphModel
    {

        public decimal Median { get; }
        public Range Dispersion { get; }
        public Range Skew { get; }

        public InputGraphModel(
            string graphId,
            string title,
            string subTitle,
            string proxiedUri,
            string ajaxUrl,
            decimal median,
            Range dispersion,
            Range skew)
            : base(graphId, title, subTitle, proxiedUri, ajaxUrl)
        {
            if (ajaxUrl == null)
            {
                throw new ArgumentNullException();
            }

            this.Median = median;
            this.Dispersion = dispersion;
            this.Skew = skew;
        }
    }
}


