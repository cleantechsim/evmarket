
using System;

namespace CleanTechSim.MainPage.Models
{

    public class IndexModel
    {

        public PreparedDataPoints EVAdoption { get; }
        public PreparedDataPoints BatteryCost { get; }
        public PreparedDataPoints EVRange { get; }
        public PreparedDataPoints EVChoice { get; }

        public IndexModel(
            PreparedDataPoints evAdoption,
            PreparedDataPoints batteryCost,
            PreparedDataPoints evRange,
            PreparedDataPoints evChoice)
        {
            this.EVAdoption = evAdoption;
            this.BatteryCost = batteryCost;
            this.EVRange = evRange;
            this.EVChoice = evChoice;
        }
    }
}