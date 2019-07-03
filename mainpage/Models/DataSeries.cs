
using System.Collections.Generic;

namespace CleanTechSim.MainPage.Models
{
    public class DataSeries
    {
        public DataSource Source { get; }

        public List<DataPoint> DataPoints { get; }


        public DataSeries(params DataPoint[] dataPoints) : this(null, dataPoints)
        {
        }

        public DataSeries(DataSource dataSource, DataPoint[] dataPoints)
        {
            this.Source = dataSource;
            this.DataPoints = new List<DataPoint>(dataPoints);
        }
    }
}