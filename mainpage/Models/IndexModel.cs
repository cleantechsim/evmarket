
using System;

using CleanTechSim.MainPage.Models.Helper.ClientGraph;

namespace CleanTechSim.MainPage.Models
{

    public class IndexModel
    {

        public PreparedDataPoints EVAdoption { get; }
        public PreparedDataPoints BatteryCost { get; }
        public PreparedDataPoints EVRange { get; }
        public PreparedDataPoints EVChoice { get; }
        public PreparedDataPoints EVPerformance { get; }
        public PreparedDataPoints EVSalesPrice { get; }

        public IndexModel(
            PreparedDataPoints evAdoption,
            PreparedDataPoints batteryCost,
            PreparedDataPoints evRange,
            PreparedDataPoints evChoice,
            PreparedDataPoints evPerformance,
            PreparedDataPoints evSalesPrice)
        {
            this.EVAdoption = evAdoption;
            this.BatteryCost = batteryCost;
            this.EVRange = evRange;
            this.EVChoice = evChoice;
            this.EVPerformance = evPerformance;
            this.EVSalesPrice = evSalesPrice;
        }
    }
}