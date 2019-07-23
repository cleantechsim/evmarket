using System;
using System.Collections.Generic;
using System.Linq;

using CleanTechSim.EVMarket.Models.Helper.GraphData;
using CleanTechSim.EVMarket.Models.Helper.GraphData.Prepare;

namespace CleanTechSim.EVMarket.Models.Helper.Graphs
{
    public abstract class StatsGraph
    {
        public abstract string Title { get; }
        public abstract string SubTitle { get; }

        internal static DataSeries GetDataSeries<T, PREPARED>(T input, IGraphModelType<T, PREPARED> graphModelType, int lineNo)
        {
            PREPARED prepared = graphModelType.Prepare(input);

            int num = graphModelType.GetNumX(input, prepared);
            List<DataPoint> dataPoints = new List<DataPoint>(num);

            for (int i = 0; i < num; ++i)
            {
                DataPoint dataPoint = new DataPoint(
                    graphModelType.GetDataPointX(input, prepared, lineNo, i),
                    graphModelType.GetDataPointY(input, prepared, lineNo, i),
                    graphModelType.GetSources(input, prepared, i)
                );

                dataPoints.Add(dataPoint);
            }

            DataSeries dataSeries = new DataSeries(null, dataPoints);

            return dataSeries;
        }

        internal static Color ColorForLine(int lineNo)
        {
            return Color.Colors.ElementAt(lineNo);
        }
    }

    internal abstract class BaseInstanceStatsGraph<INPUT, INSTANCE, PREPARED, MODEL> : StatsGraph
        where MODEL : IGraphModelType<INPUT, PREPARED>
    {

        public override string Title { get { return GraphModelType.Title; } }
        public override string SubTitle { get { return GraphModelType.SubTitle; } }

        internal MODEL GraphModelType { get; }

        internal BaseInstanceStatsGraph(MODEL graphModelType)
        {
            this.GraphModelType = graphModelType;
        }

        internal LineGraph GetAllSingleLine(INPUT elements)
        {
            DataSeries dataSeries = GetDataSeries<INPUT, PREPARED>(elements, GraphModelType, 0);

            return new LineGraph(
                GraphModelType.Title,
                GraphModelType.SubTitle,
                GraphModelType.DataPointFormat,
                new Line[] { new Line(null, Color.Green, dataSeries) });
        }

        internal LineGraph GetAllMultiLine(INPUT elements)
        {
            Line[] lines = new Line[GraphModelType.NumLines];

            for (int lineNo = 0; lineNo < GraphModelType.NumLines; ++lineNo)
            {
                DataSeries dataSeries = GetDataSeries<INPUT, PREPARED>(elements, GraphModelType, lineNo);

                string lineLabel = GraphModelType.GetLineLabel(lineNo);

                lines[lineNo] = new Line(lineLabel, ColorForLine(lineNo), dataSeries);
            }

            return new LineGraph(
                GraphModelType.Title,
                GraphModelType.SubTitle,
                GraphModelType.DataPointFormat,
                lines);
        }

    }

    internal class InstanceStatsGraph<INSTANCE, PREPARED>
        : BaseInstanceStatsGraph<IEnumerable<INSTANCE>, INSTANCE, PREPARED, IGraphModelType<IEnumerable<INSTANCE>, PREPARED>>
    {
        internal InstanceStatsGraph(IGraphModelType<IEnumerable<INSTANCE>, PREPARED> graphModelType)
            : base(graphModelType)
        {
        }

    }

    internal class KeyedInstanceStatsGraph<INSTANCE, KEY>
        : BaseInstanceStatsGraph<IEnumerable<INSTANCE>, INSTANCE, object, IMultiLineGraphModelType<INSTANCE, KEY>>
    {
        internal KeyedInstanceStatsGraph(IMultiLineGraphModelType<INSTANCE, KEY> graphModelType)
            : base(graphModelType)
        {

        }

        internal LineGraph GetAllMultiLineKeyed(IEnumerable<INSTANCE> elements)
        {

            IDictionary<KEY, List<INSTANCE>> dictionary = GraphModelType.GetByDistinctKeys(elements);

            // Add datapoints
            List<LineInfo> lineInfos = new List<LineInfo>(dictionary.Keys.Count());

            foreach (KEY key in dictionary.Keys)
            {
                DataSeries dataSeries = GetDataSeries<IEnumerable<INSTANCE>, object>(dictionary[key], GraphModelType, lineInfos.Count());

                LineInfo lineInfo = new LineInfo(GraphModelType.GetLineLabel(key), dataSeries);

                lineInfos.Add(lineInfo);
            }

            // Sort in descending order
            lineInfos.Sort((value1, value2) => -value1.CompareTo(value2));

            int num = lineInfos.Count();

            List<Line> lines = new List<Line>(num);

            for (int i = 0; i < num; ++i)
            {
                LineInfo lineInfo = lineInfos.ElementAt(i);
                Line line = new Line(
                    lineInfo.Label,
                    ColorForLine(i),
                    lineInfo.DataSeries
                );

                lines.Add(line);
            }

            return new LineGraph(
                GraphModelType.Title,
                GraphModelType.SubTitle,
                GraphModelType.DataPointFormat,
                lines);
        }
    }

    internal class LineInfo : IComparable
    {
        public string Label { get; }
        public DataSeries DataSeries { get; }

        private decimal dataSeriesSum;

        public LineInfo(string label, DataSeries dataSeries)
        {
            this.Label = label;
            this.DataSeries = dataSeries;

            this.dataSeriesSum = dataSeries.DataPoints.Sum(dataPoint => dataPoint.YValue.HasValue ? dataPoint.YValue.Value : 0m);
        }

        public int CompareTo(object obj)
        {
            LineInfo other = (LineInfo)obj;

            return this.dataSeriesSum.CompareTo(other.dataSeriesSum);
        }
    }
}