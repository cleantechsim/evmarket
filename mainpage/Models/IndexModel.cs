
using System;

namespace CleanTechSim.MainPage.Models
{

    public class IndexModel
    {

        public PreparedDataPoints EVAdoption { get; set; }

        public IndexModel(PreparedDataPoints evAdoption)
        {
            if (evAdoption == null)
            {
                throw new ArgumentNullException();
            }

            this.EVAdoption = evAdoption;
        }

    }
}