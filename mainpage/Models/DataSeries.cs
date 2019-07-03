
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
            : this(dataSource, (IEnumerable<DataPoint>)dataPoints)
        {

        }

        public DataSeries(DataSource dataSource, IEnumerable<DataPoint> dataPoints)

        {
            this.Source = dataSource;
            this.DataPoints = new List<DataPoint>(dataPoints);
        }
    }
}