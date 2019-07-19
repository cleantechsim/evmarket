
using System;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Collections.Generic;
using CleanTechSim.MainPage.Models.Helper.ClientGraph;
using Accord.Statistics.Distributions.Univariate;


[assembly: InternalsVisibleTo("mainpage_test")]
namespace CleanTechSim.MainPage.Models.Helper.GraphData.Prepare
{

    public class DynamicGraph
    {
        private decimal maxGraphXAxisTimesMedian;
        private decimal minHistogramInterval;

        public string Title { get; }
        public string SubTitle { get; }

        public decimal Median { get; }
        public decimal MinMedian { get; }
        public decimal MaxMedian { get; }

        public decimal SuggestedMaxY { get; }

        public decimal Dispersion { get; }
        public decimal Skew { get; }

        public DynamicGraph(
            string title, string subTitle,
            decimal median, decimal minMedianInput, decimal maxMedianInput,
            decimal suggestedMaxY,
            decimal maxGraphXAxisTimesMedian,
            decimal dispersion, decimal skew)
        {
            this.Title = title;
            this.SubTitle = subTitle;
            this.Median = median;
            this.MinMedian = minMedianInput;
            this.MaxMedian = maxMedianInput;
            this.SuggestedMaxY = suggestedMaxY;
            this.Dispersion = dispersion;
            this.Skew = skew;

            if (maxGraphXAxisTimesMedian < 1)
            {
                throw new ArgumentException();
            }

            this.maxGraphXAxisTimesMedian = maxGraphXAxisTimesMedian;
        }

        public const int NUM_HISTOGRAM_ENTRIES = 10;

        internal static IEnumerable<int> ToDigitsArray(int value)
        {
            List<int> digits = new List<int>(30);

            int val = value;

            while (val > 0)
            {
                int x = val % 10;

                digits.Add(x);

                val /= 10;
            }

            digits.Reverse();

            return digits;
        }

        internal static int MakeDecimal(int start, int zeroes)
        {
            int val = start;

            for (int i = 0; i < zeroes; ++i)
            {
                val *= 10;
            }

            return val;
        }

        internal static int FindNearestRounding(decimal value)
        {
            int asInt = (int)value;
            IEnumerable<int> digits = ToDigitsArray(asInt);

            int result;

            int mostSignificantDigit = digits.ElementAt(0);
            if (mostSignificantDigit > 5)
            {
                result = MakeDecimal(1, digits.Count());
            }
            else if (mostSignificantDigit == 5)
            {
                if (digits.Skip(1).Any(digit => digit != 0) || value != asInt)
                {
                    result = MakeDecimal(1, digits.Count());
                }
                else
                {
                    result = MakeDecimal(5, digits.Count() - 1);
                }
            }
            else
            {
                result = MakeDecimal(5, digits.Count() - 1);
            }

            return result;
        }

        internal static int FindGraphPercentageIntervals(decimal median, decimal maxValue, out int numIntervals)
        {
            return FindGraphPercentageIntervals(median, maxValue, NUM_HISTOGRAM_ENTRIES, out numIntervals);
        }

        internal static int FindGraphPercentageIntervals(decimal median, decimal maxValue, int numPreferedIntervals, out int numIntervals)
        {
            List<int> result = new List<int>(numPreferedIntervals);

            if (median < 1)
            {
                throw new ArgumentException();
            }

            decimal dividedByNumIntervals = median / numPreferedIntervals;
            int rounding = FindNearestRounding(dividedByNumIntervals);

            numIntervals = (int)Decimal.Round(maxValue / rounding, MidpointRounding.AwayFromZero);

            return rounding;
        }

        public PreparedDataPoints GenerateDataPoints(decimal median, decimal dispersion, decimal skew)
        {
            decimal maxGraph = median * maxGraphXAxisTimesMedian;

            SkewNormalDistribution distribution = new SkewNormalDistribution(
                (double)median,
                (double)(median * dispersion),
                (double)skew);

            int numIntervals;

            int step = FindGraphPercentageIntervals(median, maxGraph, out numIntervals);

            string[] labels = new string[numIntervals];
            List<decimal?> decimals = new List<decimal?>(numIntervals);

            int startInterval = 0;

            // Make sure not to include distribution data at negative x offset.
            // This is due to normal distribution not appropriate in all cases
            decimal xValue = startInterval * step;
            decimal lastValue = (decimal)distribution.DistributionFunction((double)xValue);

            for (int i = startInterval; i < numIntervals; ++i)
            {
                decimal computeXValue = xValue + step / 2;
                labels[i] = "" + computeXValue;

                decimal distValue = (decimal)distribution.DistributionFunction((double)computeXValue);

                xValue += step;

                decimal fractionOfX = 1 / (decimal)numIntervals;

                // ?? sometimes occurs, but not significant
                decimal diff;
                if (distValue <= lastValue)
                {
                    diff = 0;
                }
                else
                {
                    diff = distValue - lastValue;
                    lastValue = distValue;
                }

                decimals.Add(diff / fractionOfX);
            }

            DataSet dataSet = new DataSet(null, Color.Green, decimals);

            return new PreparedDataPoints(labels, new DataSet[] { dataSet });
        }
    }

    public class DynamicData
    {
        public static DynamicGraph IncomeGraph = new DynamicGraph(
                    "Income",
                    "Income distribution",
                    25000m,
                    1000m,
                    100000m,
                    10.0m,
                    3.0m,
                    0.5m,
                    0m);

    }
}