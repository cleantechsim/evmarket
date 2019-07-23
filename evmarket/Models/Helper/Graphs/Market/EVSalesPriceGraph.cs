using System;

using System.Collections.Generic;
using System.Linq;

using CleanTechSim.MainPage.Helpers;

using CleanTechSim.MainPage.Models.Domain;

using CleanTechSim.MainPage.Models.Helper.GraphData;
using CleanTechSim.MainPage.Models.Helper.GraphData.Prepare;

namespace CleanTechSim.MainPage.Models.Helper.Graphs.Market
{

    internal class EVSalesPricePrepared : EVYearsPrepared
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

    internal class EVSalesPriceGraph : YearsGraph<EVSalesPricePrepared>
    {
        internal static EVSalesPricePrepared ComputeAverageAndMedianSalesPricePerYear(IEnumerable<Vehicle> instances)
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

        private static IGraphModelType<IEnumerable<Vehicle>, EVSalesPricePrepared> MODEL
            = new GraphModelType<IEnumerable<Vehicle>, EVSalesPricePrepared>(
            "Cost",
            "Sales price in dollars",

            Encoding.YEAR,
            Encoding.NUMBER,

            2,

            instances => ComputeAverageAndMedianSalesPricePerYear(instances),

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

        internal static readonly EVSalesPriceGraph INSTANCE = new EVSalesPriceGraph();

        private EVSalesPriceGraph()
            : base(MODEL)
        {

        }
    }

}
