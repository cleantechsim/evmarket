
using System;

namespace CleanTechSim.MainPage.Models
{
    public class IndexModel
    {
        public StatsGraphModel EVAdoption { get; }
        public StatsGraphModel BatteryCost { get; }
        public StatsGraphModel EVRange { get; }
        public StatsGraphModel EVChoice { get; }
        public StatsGraphModel EVPerformance { get; }
        public StatsGraphModel EVSalesPrice { get; }

        public InputGraphModel Income { get; }
        public InputGraphModel RangeRequirement { get; }
        public InputGraphModel Propensity { get; }

        public IndexModel(
            StatsGraphModel evAdoption,
            StatsGraphModel batteryCost,
            StatsGraphModel evRange,
            StatsGraphModel evChoice,
            StatsGraphModel evPerformance,
            StatsGraphModel evSalesPrice,

            InputGraphModel income,
            InputGraphModel rangeRequirement,
            InputGraphModel propensity)
        {
            this.EVAdoption = evAdoption;
            this.BatteryCost = batteryCost;
            this.EVRange = evRange;
            this.EVChoice = evChoice;
            this.EVPerformance = evPerformance;
            this.EVSalesPrice = evSalesPrice;

            this.Income = income;
            this.RangeRequirement = rangeRequirement;
            this.Propensity = propensity;
        }
    }
}