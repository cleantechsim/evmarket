

using System.Collections.Generic;
using System.Linq;

namespace CleanTechSim.MainPage.Models.Helper.GraphData
{
    public class DataPoint
    {
        public decimal XValue { get; }

        public decimal? YValue { get; }

        public List<DataSource> Sources { get; set; }


        public DataPoint()
        {

        }

        public DataPoint(decimal x, decimal? y) : this(x, y, (DataSource[])null)
        {

        }

        private static DataSource[] toDataSoures(string[] sourceUrls)
        {
            return sourceUrls.Select(url => new DataSource(url)).ToArray();
        }

        public DataPoint(decimal x, decimal y, params string[] sourceUrls)
            : this(x, y, toDataSoures(sourceUrls))
        {

        }

        public DataPoint(decimal x, decimal y, params DataSource[] sources)
            : this(x, y, (IEnumerable<DataSource>)sources)
        {

        }

        public DataPoint(decimal x, decimal? y, IEnumerable<DataSource> sources)
        {
            this.XValue = x;
            this.YValue = y;
            this.Sources = sources != null ? new List<DataSource>(sources) : null;
        }
    }
}