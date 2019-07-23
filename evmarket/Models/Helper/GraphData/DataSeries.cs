
using System.Collections.Generic;

namespace CleanTechSim.EVMarket.Models.Helper.GraphData
{
    public class DataSeries
    {
        public DataSource Source { get; }
        public List<DataPoint> DataPoints { get; }

        public DataSeries(params DataPoint[] dataPoints) : this((DataSource)null, dataPoints)
        {
        }

        public DataSeries(string dataSource, params DataPoint[] dataPoints)
            : this(new DataSource(dataSource), dataPoints)
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