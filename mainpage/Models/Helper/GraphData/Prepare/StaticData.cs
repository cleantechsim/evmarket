
using System;
using System.Collections.Generic;
using System.Linq;

using CleanTechSim.MainPage.Helpers;
using CleanTechSim.MainPage.Models.Domain;

namespace CleanTechSim.MainPage.Models.Helper.GraphData.Prepare
{
    public class StaticData
    {
        public static IMultiLineGraphModelType<MonthlyCountryEVCarSales, string> EvAdoptionGraph = new MultiLineKeyedGraphModelType<MonthlyCountryEVCarSales, string>(
            "Awareness",
            "EV percent marketshare by month",

            Encoding.YEAR_MONTH,
            Encoding.NUMBER,

            instance => instance.Year + ((decimal)instance.Month) / 100,
            instance => instance.SalesPercent,

            instance => instance.Country,
            key => key
        );

        public static ISingleLineGraphModelType<BatteryCost> BatteryCostGraph = new SingleLineGraphModelType<BatteryCost>(
            "Cost",
            "Battery cost in $/kWh",

            Encoding.YEAR,
            Encoding.NUMBER,

            instance => instance.Century * 100 + instance.Year,
            instance => instance.Cost
        );


        private static List<int> GetDistinctSortedYearsForVehicles(IEnumerable<Vehicle> instances, int? maxYear = null)
        {
            HashSet<int> distinctYears = new HashSet<int>();

            foreach (Vehicle v in instances)
            {
                if (v.Year.HasValue && (!maxYear.HasValue || v.Year.Value <= maxYear.Value))
                {
                    distinctYears.Add(v.Year.Value);
                }
            }

            List<int> sortedYears = new List<int>(distinctYears);

            sortedYears.Sort();

            return sortedYears;
        }

        public class EVYearsPrepared
        {
            internal IEnumerable<int> YearsSorted { get; }

            internal EVYearsPrepared(IEnumerable<int> yearsSorted)
            {
                this.YearsSorted = yearsSorted;
            }
        }

        public class EVRangePrepared : EVYearsPrepared
        {
            internal IDictionary<int, int> AverageRangeByYear { get; }
            internal IDictionary<int, int> MedianRangeByYear { get; }

            internal EVRangePrepared(IEnumerable<int> yearsSorted, IDictionary<int, int> averageRangePerYear, IDictionary<int, int> medianRangePerYear)
                : base(yearsSorted)
            {
                this.AverageRangeByYear = averageRangePerYear;
                this.MedianRangeByYear = medianRangePerYear;
            }
        }


        internal static EVRangePrepared ComputeAverageAndMedianRangePerYear(IEnumerable<Vehicle> instances)
        {
            Dictionary<Vehicle, int> wltpForCar = Vehicle.ComputeVehicleToWLTPRange(instances);
            List<int> sortedYears = GetDistinctSortedYearsForVehicles(instances);

            // Make dictionary of average by year
            Dictionary<int, int> averageRangeByYear = new Dictionary<int, int>();
            Dictionary<int, int> medianRangeByYear = new Dictionary<int, int>();

            foreach (int year in sortedYears)
            {
                IEnumerable<Vehicle> yearVehicles = from Vehicle v in instances
                                                    where
                                                           v.Year != null
                                                        && v.Year.Value == year
                                                    select v;

                averageRangeByYear[year] = (int)(from Vehicle v in yearVehicles
                                                 where wltpForCar.ContainsKey(v)
                                                 select wltpForCar[v]).Average();

                medianRangeByYear[year] = (int)(from Vehicle v in yearVehicles
                                                where wltpForCar.ContainsKey(v)
                                                select wltpForCar[v]).Median();
            }

            return new EVRangePrepared(sortedYears, averageRangeByYear, medianRangeByYear);
        }

        public static IGraphModelType<IEnumerable<Vehicle>, EVRangePrepared> EVRangeGraph
            = new GraphModelType<IEnumerable<Vehicle>, EVRangePrepared>(
            "Range",
            "EV average range for new models",

            Encoding.YEAR,
            Encoding.NUMBER,

            2,

            instances => ComputeAverageAndMedianRangePerYear(instances),

            line => line == 0 ? "Average" : "Median",

            (instances, prepared) => prepared.YearsSorted.Count(),

            (instances, prepared, line, index) => prepared.YearsSorted.ElementAt(index),
            (instances, prepared, line, index) =>
            {
                int year = prepared.YearsSorted.ElementAt(index);

                return line == 0
                    ? prepared.AverageRangeByYear[year]
                    : prepared.MedianRangeByYear[year];
            }
        );

        public class EVChoicePrepared : EVYearsPrepared
        {
            internal IDictionary<int, int> NumberOfNewModelsByYear { get; }

            internal EVChoicePrepared(IEnumerable<int> yearsSorted, IDictionary<int, int> numberOfModelsByYear)
                : base(yearsSorted)
            {
                this.NumberOfNewModelsByYear = numberOfModelsByYear;
            }
        }

        internal static EVChoicePrepared ComputeNumberOfNewModelsPerYear(IEnumerable<Vehicle> instances)
        {
            List<int> sortedYears = GetDistinctSortedYearsForVehicles(instances, DateTime.Now.Year);

            // Make dictionary of average by year
            Dictionary<int, int> numberOfNewModelsByYear = new Dictionary<int, int>();

            foreach (int year in sortedYears)
            {
                IEnumerable<Vehicle> yearVehicles = from Vehicle v in instances
                                                    where
                                                           v.Year != null
                                                        && v.Year.Value == year
                                                    select v;

                numberOfNewModelsByYear[year] = yearVehicles.Count();
            }
            return new EVChoicePrepared(sortedYears, numberOfNewModelsByYear);
        }

        public static IGraphModelType<IEnumerable<Vehicle>, EVChoicePrepared> EVChoiceGraph
            = new GraphModelType<IEnumerable<Vehicle>, EVChoicePrepared>(
            "Choice",
            "Number of new models per year",

            Encoding.YEAR,
            Encoding.INTEGER,

            1,

            instances => ComputeNumberOfNewModelsPerYear(instances),

            null,

            (instances, prepared) => prepared.YearsSorted.Count(),

            (instances, prepared, line, index) => prepared.YearsSorted.ElementAt(index),
            (instances, prepared, line, index) => prepared.NumberOfNewModelsByYear[prepared.YearsSorted.ElementAt(index)]
        );


        public class EVPerformancePrepared : EVYearsPrepared
        {
            internal IDictionary<int, decimal?> AveragePerformanceByYear { get; }

            internal EVPerformancePrepared(IEnumerable<int> yearsSorted, IDictionary<int, decimal?> averagePerformancePerYear)
                : base(yearsSorted)
            {
                this.AveragePerformanceByYear = averagePerformancePerYear;
            }
        }

        internal static EVPerformancePrepared ComputeAveragePerformancePerYear(IEnumerable<Vehicle> instances)
        {
            List<int> sortedYears = GetDistinctSortedYearsForVehicles(instances);

            // Make dictionary of average by year
            Dictionary<int, decimal?> averagePerformanceByYear = new Dictionary<int, decimal?>();

            foreach (int year in sortedYears)
            {
                IEnumerable<Vehicle> yearVehiclesWithAcceleration = from Vehicle v in instances
                                                                    where
                                                                           v.Year != null
                                                                        && v.Year.Value == year
                                                                        && v.Acceleration.HasValue
                                                                    select v;

                averagePerformanceByYear[year] = yearVehiclesWithAcceleration.Any()
                        ? Math.Round(
                            yearVehiclesWithAcceleration.Average(v => v.Acceleration.Value),
                            1,
                            MidpointRounding.AwayFromZero)
                        : default(decimal?);

            }
            return new EVPerformancePrepared(sortedYears, averagePerformanceByYear);
        }

        public static IGraphModelType<IEnumerable<Vehicle>, EVPerformancePrepared> EVPerformanceGraph
            = new GraphModelType<IEnumerable<Vehicle>, EVPerformancePrepared>(
            "0-100 km/t (0-60 mph)",
            "Performance",

            Encoding.YEAR,
            Encoding.NUMBER,

            1,

            instances => ComputeAveragePerformancePerYear(instances),

            null,

            (instances, prepared) => prepared.YearsSorted.Count(),

            (instances, prepared, line, index) => prepared.YearsSorted.ElementAt(index),
            (instances, prepared, line, index) =>
            {
                int year = prepared.YearsSorted.ElementAt(index);

                decimal? accelleration = prepared.AveragePerformanceByYear[year];

                return accelleration;
            }
        );

        public class EVSalesPricePrepared : EVYearsPrepared
        {
            internal IDictionary<int, decimal?> AveragePriceByYear { get; }
            internal IDictionary<int, decimal?> MedianPriceByYear { get; }

            internal EVSalesPricePrepared(
                IEnumerable<int> yearsSorted,
                IDictionary<int, decimal?> averagePricePerYear,
                IDictionary<int, decimal?> medianPricePerYear)
                : base(yearsSorted)
            {
                this.AveragePriceByYear = averagePricePerYear;
                this.MedianPriceByYear = medianPricePerYear;
            }
        }

        internal static EVSalesPricePrepared ComputeAverageSalesPricePerYear(IEnumerable<Vehicle> instances)
        {
            List<int> sortedYears = GetDistinctSortedYearsForVehicles(instances);

            // Make dictionary of average by year
            Dictionary<int, decimal?> averageSalesPriceByYear = new Dictionary<int, decimal?>();
            Dictionary<int, decimal?> medianSalesPriceByYear = new Dictionary<int, decimal?>();

            foreach (int year in sortedYears)
            {
                IEnumerable<Vehicle> yearVehiclesWithSalesPrice = from Vehicle v in instances
                                                                  where
                                                                         v.Year != null
                                                                      && v.Year.Value == year
                                                                      && v.PriceDollars.HasValue
                                                                  select v;

                averageSalesPriceByYear[year] = yearVehiclesWithSalesPrice.Any()
                        ? Math.Round(
                            yearVehiclesWithSalesPrice.Average(v => v.PriceDollars.Value),
                            1,
                            MidpointRounding.AwayFromZero)
                        : default(decimal?);

                medianSalesPriceByYear[year] = yearVehiclesWithSalesPrice.Any()
                        ? Math.Round(
                            yearVehiclesWithSalesPrice.Median(v => v.PriceDollars.Value),
                            1,
                            MidpointRounding.AwayFromZero)
                        : default(decimal?);

            }
            return new EVSalesPricePrepared(sortedYears, averageSalesPriceByYear, medianSalesPriceByYear);
        }

        public static IGraphModelType<IEnumerable<Vehicle>, EVSalesPricePrepared> EVSalesPriceGraph
            = new GraphModelType<IEnumerable<Vehicle>, EVSalesPricePrepared>(
            "Cost",
            "Sales price in dollars",

            Encoding.YEAR,
            Encoding.NUMBER,

            2,

            instances => ComputeAverageSalesPricePerYear(instances),

            line => line == 0 ? "Average" : "Median",

            (instances, prepared) => prepared.YearsSorted.Count(),

            (instances, prepared, line, index) => prepared.YearsSorted.ElementAt(index),
            (instances, prepared, line, index) =>
            {
                int year = prepared.YearsSorted.ElementAt(index);

                decimal? price = line == 0
                    ? prepared.AveragePriceByYear[year]
                    : prepared.MedianPriceByYear[year];

                return price;
            }
        );
    }
}

