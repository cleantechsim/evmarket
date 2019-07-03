
using System;
using System.Collections.Generic;
using System.Linq;

namespace CleanTechSim.MainPage.Models
{
    public class PreparedDataPoints
    {
        private readonly string graphId;

        private readonly string title;

        private readonly List<string> labels;
        private readonly List<DataSet> dataSets;

        public string GraphId { get { return graphId; } }

        public string Title { get { return title; } }

        public IEnumerable<string> Labels { get { return labels; } }

        public IEnumerable<DataSet> DataSets { get { return dataSets; } }

        private PreparedDataPoints(string graphId, string title, List<string> labels, List<DataSet> dataSets)
        {
            if (graphId == null)
            {
                throw new ArgumentNullException();
            }

            if (labels == null)
            {
                throw new ArgumentNullException();
            }

            if (dataSets == null)
            {
                throw new ArgumentNullException();
            }

            this.graphId = graphId;
            this.title = title;
            this.labels = labels;
            this.dataSets = dataSets;
        }


        private static HashSet<decimal> GetDistinctXValuesForAllDataSeries(IEnumerable<DataSeries> dataSeries)
        {

            HashSet<decimal> allDistinctValues = new HashSet<decimal>();

            foreach (DataSeries series in dataSeries)
            {
                decimal? lastXValue = null;

                foreach (DataPoint datapoint in series.DataPoints)
                {

                    if (lastXValue != null)
                    {
                        if (datapoint.XValue < lastXValue)
                        {
                            throw new ArgumentException("Expected incrementing order");
                        }

                    }
                    lastXValue = datapoint.XValue;

                    allDistinctValues.Add(datapoint.XValue);
                }
            }

            return allDistinctValues;
        }

        private static List<string> ComputeLabelNames(DataPointFormat format, decimal minXValue, decimal maxXValue, ref List<decimal> allPossibleValues)
        {
            List<string> labels = new List<string>();

            const int MONTH_MULTIPLICATOR = 100;

            allPossibleValues = new List<decimal>();

            switch (format.XEncoding)
            {
                case Encoding.YEAR_MONTH:
                    // Add from labels
                    int minYear = (int)minXValue;
                    int minMonth = (int)(MONTH_MULTIPLICATOR * (minXValue - minYear));

                    int maxYear = (int)maxXValue;
                    int maxMonth = (int)(MONTH_MULTIPLICATOR * (maxXValue - maxYear));

                    for (int year = minYear; year <= maxYear; ++year)
                    {
                        for (int month = minMonth;
                            (year == maxYear && month <= maxMonth)
                                || (year < maxYear && month <= 12);
                            ++month)
                        {

                            labels.Add(string.Format("{0}/{1}", year, month));

                            decimal monthDecimal = month;

                            allPossibleValues.Add(year + (monthDecimal / MONTH_MULTIPLICATOR));

                        }

                        // Start at 1st month in next iteration
                        minMonth = 1;
                    }
                    break;

                default:
                    throw new InvalidOperationException();
            }

            return labels;
        }
        public static PreparedDataPoints VerifyAndCompute(string graphId, LineGraph lineGraph)
        {
            return VerifyAndCompute(graphId, lineGraph.Title, lineGraph.Lines, lineGraph.DataPointFormat);
        }

        private static PreparedDataPoints VerifyAndCompute(string graphId, string title, IEnumerable<Line> graphLines, DataPointFormat format)
        {

            // Find the y-values and the labels for those

            HashSet<decimal> allDistinctValues = GetDistinctXValuesForAllDataSeries(graphLines.Select(line => line.DataSeries));

            List<decimal> sortedValues = new List<decimal>(allDistinctValues);
            sortedValues.Sort();


            decimal minXValue = sortedValues[0];
            decimal maxXValue = sortedValues[sortedValues.Count - 1];

            List<decimal> allPossibleXValues = null;

            List<string> labels = ComputeLabelNames(format, minXValue, maxXValue, ref allPossibleXValues);

            List<DataSet> dataSets = new List<DataSet>(labels.Count);

            foreach (Line line in graphLines)
            {

                DataSeries series = line.DataSeries;

                int seriesIdx = 0;

                List<decimal?> dataSetValues = new List<decimal?>();

                foreach (decimal value in allPossibleXValues)
                {
                    if (seriesIdx < series.DataPoints.Count)
                    {
                        DataPoint dataPoint = series.DataPoints[seriesIdx];
                        decimal dataPointXValue = dataPoint.XValue;

                        if (dataPointXValue == value)
                        {
                            dataSetValues.Add(dataPoint.YValue);

                            ++seriesIdx;
                        }
                        else
                        {
                            dataSetValues.Add(null);
                        }
                    }
                    else
                    {
                        dataSetValues.Add(null);
                    }
                }

                if (seriesIdx != series.DataPoints.Count)
                {
                    throw new InvalidOperationException();
                }

                if (dataSetValues.Count != allPossibleXValues.Count)
                {
                    throw new InvalidOperationException();
                }

                dataSets.Add(new DataSet(line.Label, line.Color, dataSetValues));
            }


            return new PreparedDataPoints(graphId, title, labels, dataSets);

        }

    }

}