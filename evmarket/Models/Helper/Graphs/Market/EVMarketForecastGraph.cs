using System;

using System.Collections.Generic;
using System.Linq;

using CleanTechSim.EVMarket.Helpers;

using CleanTechSim.EVMarket.Helpers.Compute;

using CleanTechSim.EVMarket.Models.Domain;
using CleanTechSim.EVMarket.Models.Helper.GraphData;
using CleanTechSim.EVMarket.Models.Helper.GraphData.Prepare;

using CleanTechSim.EVMarket.Models.Helper.Graphs.Consumer;

namespace CleanTechSim.EVMarket.Models.Helper.Graphs.Market
{
    internal class ForecastInput
    {
        internal IEnumerable<Vehicle> Vehicles { get; }

        internal InputGraphSelection IncomeGraphSelection { get; }

        internal decimal PercentageOfWageForCarCost { get; }

        internal ForecastInput(IEnumerable<Vehicle> vehicles, InputGraphSelection incomeGraphSelection, decimal percentageOfWageForCarCost)
        {
            this.Vehicles = vehicles;
            this.IncomeGraphSelection = incomeGraphSelection;
            this.PercentageOfWageForCarCost = percentageOfWageForCarCost;
        }
    }

    internal class EVMarketForecastPrepared : EVYearsPrepared
    {
        internal IDictionary<int, decimal> MarketSharePercentByYear { get; }

        internal EVMarketForecastPrepared(IEnumerable<int> yearsSorted, IDictionary<int, decimal> marketSharePercentByYear)
            : base(yearsSorted)
        {
            this.MarketSharePercentByYear = marketSharePercentByYear;
        }
    }

    internal class EVMarketForecastGraph : BaseInstanceStatsGraph<
                                                        ForecastInput,
                                                        Vehicle,
                                                        EVMarketForecastPrepared,
                                                        IGraphModelType<ForecastInput, EVMarketForecastPrepared>>
    {
        private const int YEARS = 10;

        private static EVMarketForecastPrepared ComputeMarketForecast(ForecastInput input, int numYears)
        {
            var thisYear = DateTime.Now.Year;

            // Find the median car cost up till last years model
            EVSalesPricePrepared salesPrices = EVSalesPriceGraph.ComputeAverageAndMedianSalesPricePerYear(input.Vehicles);
            decimal averageOfSalesPriceMedian = salesPrices.MedianPriceByYear.Values
                        .Where(value => value.HasValue)
                        .Average(value => value.Value);

            Func<decimal, decimal> computePurchaseProbabilityFromIncome = salesPrice =>
                {
                    decimal incomeThreshold = salesPrice * 100 / input.PercentageOfWageForCarCost;

                    decimal probabilityAbove = IncomeGraph.INSTANCE.ComputeProbabilityAboveX(
                        input.IncomeGraphSelection,
                        incomeThreshold);

                    return probabilityAbove;
                };

            IEnumerable<decimal> values = MarketForecaster.ProduceYearlyForecast(
                numYears,
                averageOfSalesPriceMedian,
                input.PercentageOfWageForCarCost,
                computePurchaseProbabilityFromIncome,
                50m,

                // yearly cost reduction in percent
                5m

            );

            Dictionary<int, decimal> forecastByYear = new Dictionary<int, decimal>(numYears);

            int year = thisYear;

            foreach (decimal value in values)
            {
                forecastByYear[year++] = value;
            }

            IEnumerable<int> yearsSorted = Enumerable.Range(thisYear, numYears);

            return new EVMarketForecastPrepared(yearsSorted, forecastByYear);
        }


        private static readonly IGraphModelType<ForecastInput, EVMarketForecastPrepared> MODEL
                = new GraphModelType<ForecastInput, EVMarketForecastPrepared>(
                "Market forecast",
                "Forcast for next years, with cost reductions",

                Encoding.YEAR,
                Encoding.NUMBER,

                1,

                input => ComputeMarketForecast(input, YEARS),

                null,

                (instances, prepared) => prepared.YearsSorted.Count(),

                (instances, prepared, line, index) => prepared.YearsSorted.ElementAt(index),
                (instances, prepared, line, index) =>
                {
                    int year = prepared.YearsSorted.ElementAt(index);

                    decimal? accelleration = prepared.MarketSharePercentByYear[year];

                    return accelleration;
                }
            );

        internal static readonly EVMarketForecastGraph INSTANCE = new EVMarketForecastGraph();

        private EVMarketForecastGraph()
            : base(MODEL)
        {

        }
    }
}

