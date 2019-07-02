
using System.Collections.Generic;

namespace CleanTechSim.MainPage.Models
{
    public class DataSeries
    {
        public DataSeries(params DataPoint[] dataPoints) : this(null, dataPoints)
        {
        }

        public DataSeries(DataSource dataSource, DataPoint[] dataPoints)
        {
            this.Source = dataSource;
            this.DataPoints = new List<DataPoint>(dataPoints);
        }

        public int Id { get; set; }

        public DataSource Source { get; set; }

        public List<DataPoint> DataPoints { get; set; }

    }
}