
using System;

namespace CleanTechSim.MainPage.Models
{

    public class IndexModel
    {

        public PreparedDataPoints EVAdoption { get; set; }
        public PreparedDataPoints BatteryCost { get; }

        public IndexModel(PreparedDataPoints evAdoption, PreparedDataPoints batteryCost)
        {
            if (evAdoption == null)
            {
                throw new ArgumentNullException();
            }

            this.EVAdoption = evAdoption;
            this.BatteryCost = batteryCost;
        }

    }
}