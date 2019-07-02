

using System.Collections.Generic;
using System.Linq;

namespace CleanTechSim.MainPage.Models
{

    public class DataPoint
    {

        public DataPoint()
        {

        }

        public DataPoint(decimal x, decimal y) : this(x, y, (DataSource[])null)
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
        {
            this.XValue = x;
            this.YValue = y;
            this.Sources = new List<DataSource>(sources);
        }

        public int Id { get; set; }

        public decimal XValue { get; set; }

        public decimal YValue { get; set; }

        public List<DataSource> Sources { get; set; }
    }
}