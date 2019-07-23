
namespace CleanTechSim.EVMarket.Models.Helper.GraphData
{

    public class DataPointFormat
    {
        public Encoding XEncoding { get; }
        public Encoding YEncoding { get; }

        public DataPointFormat(Encoding xEncoding, Encoding yEncoding)
        {
            this.XEncoding = xEncoding;
            this.YEncoding = yEncoding;
        }
    }
}