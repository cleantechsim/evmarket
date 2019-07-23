

namespace CleanTechSim.EVMarket.Models.Helper.GraphData
{
    public class DataSource
    {

        public int Id { get; set; }

        public string Url { get; set; }

        public DataSource()
        {

        }

        public DataSource(string url)
        {
            this.Url = url;
        }

    }
}

