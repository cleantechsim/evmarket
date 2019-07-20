
using System;

namespace CleanTechSim.MainPage.Models
{
    public class IndexModel
    {
        public StaticGraphModel EVAdoption { get; }
        public StaticGraphModel BatteryCost { get; }
        public StaticGraphModel EVRange { get; }
        public StaticGraphModel EVChoice { get; }
        public StaticGraphModel EVPerformance { get; }
        public StaticGraphModel EVSalesPrice { get; }

        public DynamicGraphModel Income { get; }
        public DynamicGraphModel RangeRequirement { get; }
        public DynamicGraphModel Propensity { get; }

        public IndexModel(
            StaticGraphModel evAdoption,
            StaticGraphModel batteryCost,
            StaticGraphModel evRange,
            StaticGraphModel evChoice,
            StaticGraphModel evPerformance,
            StaticGraphModel evSalesPrice,

            DynamicGraphModel income,
            DynamicGraphModel rangeRequirement,
            DynamicGraphModel propensity)
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