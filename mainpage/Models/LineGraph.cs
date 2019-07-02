
using System.Collections.Generic;

namespace CleanTechSim.MainPage.Models
{

    public class LineGraph
    {

        public LineGraph()
        {

        }

        public LineGraph(params Line[] lines)
        {
            this.Lines = new List<Line>(lines);
        }

        public List<Line> Lines { get; set; }
    }

    public class Line
    {
        public string Label { get; set; }
        public Color Color { get; set; }
        /* 
                public List<string> XLabels;

                public List<string> YLabels;
        */
        public DataSeries DataSeries { get; set; }

        public Line()
        {

        }

        public Line(string label, Color color, /*  List<string> xLabels, List<string> yLabels ,*/ DataSeries dataSeries)
        {
            this.Label = label;
            this.Color = color;
            /*
            this.XLabels = xLabels;
            this.YLabels = yLabels;
             */
            this.DataSeries = dataSeries;
        }


    }

}
