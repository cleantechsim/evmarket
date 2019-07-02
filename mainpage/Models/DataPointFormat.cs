
namespace CleanTechSim.MainPage.Models
{

    public class DataPointFormat
    {
        private readonly Encoding xEncoding;
        private readonly Encoding yEncoding;

        public Encoding XEncoding { get { return xEncoding; } }
        public Encoding YEncoding { get { return yEncoding; } }

        public DataPointFormat(Encoding xEncoding, Encoding yEncoding)
        {
            this.xEncoding = xEncoding;
            this.yEncoding = yEncoding;
        }

    }
}