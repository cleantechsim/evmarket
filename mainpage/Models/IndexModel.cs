
using System;

namespace CleanTechSim.MainPage.Models
{

    public class IndexModel
    {

        public PreparedDataPoints EVAdoption { get; }
        public PreparedDataPoints BatteryCost { get; }
        public PreparedDataPoints EVRange { get; }

        public IndexModel(PreparedDataPoints evAdoption, PreparedDataPoints batteryCost, PreparedDataPoints evRange)
        {
            if (evAdoption == null)
            {
                throw new ArgumentNullException();
            }

            this.EVAdoption = evAdoption;
            this.BatteryCost = batteryCost;
            this.EVRange = evRange;
        }
    }
}