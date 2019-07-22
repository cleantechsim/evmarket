
using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Statistics.Distributions.Univariate;

using CleanTechSim.MainPage.Models.Helper.ClientGraph;
using CleanTechSim.MainPage.Models.Helper.GraphData;

using CleanTechSim.MainPage.Helpers;

namespace CleanTechSim.MainPage.Models.Helper.Graphs
{
    public class InputGraphSelection
    {
        public decimal Median { get; }
        public decimal Dispersion { get; }
        public decimal Skew { get; }

        public InputGraphSelection(decimal median, decimal dispersion, decimal skew)
        {
            this.Median = median;
            this.Dispersion = dispersion;
            this.Skew = skew;
        }
    }

    public class InputGraph
    {
        public static Range DISPERSION_DEFAULT = new Range(0, 0.75m, 1.5m, 0.05m);
        public static Range SKEW_DEFAULT = new Range(-30, 0, 30, 0.5m);

        private decimal maxGraphXAxisTimesMedian;
        private decimal minHistogramInterval;

        public string Title { get; }
        public string SubTitle { get; }

        public decimal Median { get; }
        public decimal MinMedian { get; }
        public decimal MaxMedian { get; }

        public decimal SuggestedMaxY { get; }

        public decimal? MaxXValue { get; }

        public Range Dispersion { get; }
        public Range Skew { get; }

        public InputGraph(
            string title, string subTitle,
            decimal median, decimal minMedianInput, decimal maxMedianInput,
            decimal suggestedMaxY,
            decimal maxGraphXAxisTimesMedian,
            Range dispersion, Range skew,
            decimal? maxXValue = null)
        {
            this.Title = title;
            this.SubTitle = subTitle;
            this.Median = median;
            this.MinMedian = minMedianInput;
            this.MaxMedian = maxMedianInput;
            this.SuggestedMaxY = suggestedMaxY;
            this.Dispersion = dispersion;
            this.Skew = skew;
            this.MaxXValue = maxXValue;

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
            else if (mostSignificantDigit == 1)
            {
                if (digits.Skip(1).Any(digit => digit != 0) || value != asInt)
                {
                    result = MakeDecimal(5, digits.Count() - 1);
                }
                else
                {
                    result = MakeDecimal(1, digits.Count() - 1);
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

            decimal dividedByNumIntervals = maxValue / numPreferedIntervals;
            int rounding = FindNearestRounding(dividedByNumIntervals);

            numIntervals = (int)Decimal.Round(maxValue / rounding, MidpointRounding.AwayFromZero);

            return rounding;
        }

        private static decimal FindDistributionLocation(decimal median, decimal dispersion, decimal skew)
        {
            decimal location;

            decimal acceptableRange = median / 50;

            if (skew == 0)
            {
                location = median;
            }
            else
            {
                Compare compare = value =>
                {

                    SkewNormalDistribution distribution = MakeSkewNormalDistribution(
                        value,
                        median,
                        dispersion,
                        skew);

                    decimal distMedian = (decimal)distribution.Median;

                    int cmp;
                    if (Math.Abs(distMedian - median) < acceptableRange)
                    {
                        cmp = 0;

                    }
                    else if (distMedian > median)
                    {
                        cmp = -1;
                    }
                    else
                    {
                        cmp = 1;
                    }

                    return cmp;
                };

                location = BinarySearch.Search(0, median * 5, compare);
            }

            return location;
        }

        private static SkewNormalDistribution MakeSkewNormalDistribution(decimal location, decimal median, decimal dispersion, decimal skew)
        {
            SkewNormalDistribution distribution = new SkewNormalDistribution(
                            (double)location,
                            (double)(median * dispersion),
                            (double)skew);

            return distribution;
        }

        public PreparedDataPoints GenerateDataPoints(decimal median, decimal dispersion, decimal skew, decimal? maxXValue)
        {
            decimal maxGraph = median * maxGraphXAxisTimesMedian;

            if (maxXValue.HasValue && maxXValue.Value < maxGraph)
            {
                maxGraph = maxXValue.Value;
            }

            decimal location = FindDistributionLocation(median, dispersion, skew);

            SkewNormalDistribution distribution = MakeSkewNormalDistribution(
                location,
                median,
                dispersion,
                skew);

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
                decimal computeXValue = xValue + step / 2m;
                labels[i] = "" + xValue;

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

        public decimal ComputeProbabilityAboveX(InputGraphSelection selection, decimal x)
        {
            decimal location = FindDistributionLocation(selection.Median, selection.Dispersion, selection.Skew);

            SkewNormalDistribution distribution = MakeSkewNormalDistribution(
                location,
                selection.Median,
                selection.Dispersion,
                selection.Skew);

            decimal max = location * 100m;

            return (decimal)(distribution.DistributionFunction((double)max) - distribution.DistributionFunction((double)x));
        }
    }
}