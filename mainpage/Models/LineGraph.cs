
using System.Collections.Generic;

namespace CleanTechSim.MainPage.Models
{

    public class LineGraph
    {
        public string Title { get; }
        public DataPointFormat DataPointFormat { get; }
        public IEnumerable<Line> Lines { get; set; }

        public LineGraph()
        {

        }

        public LineGraph(string title, DataPointFormat dataPointFormat, params Line[] lines)
            : this(title, dataPointFormat, (IEnumerable<Line>)lines)
        {

        }

        public LineGraph(string title, DataPointFormat dataPointFormat, IEnumerable<Line> lines)
        {
            this.Title = title;
            this.DataPointFormat = dataPointFormat;
            this.Lines = new List<Line>(lines);
        }
    }

    public class Line
    {
        public string Label { get; }
        public Color Color { get; }
        public DataSeries DataSeries { get; }

        public Line()
        {

        }

        public Line(string label, Color color, DataSeries dataSeries)
        {
            this.Label = label;
            this.Color = color;
            this.DataSeries = dataSeries;
        }
    }
}
