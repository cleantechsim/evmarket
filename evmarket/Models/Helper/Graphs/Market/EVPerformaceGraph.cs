using System;

using System.Collections.Generic;
using System.Linq;

using CleanTechSim.EVMarket.Models.Domain;

using CleanTechSim.EVMarket.Models.Helper.GraphData;
using CleanTechSim.EVMarket.Models.Helper.GraphData.Prepare;

namespace CleanTechSim.EVMarket.Models.Helper.Graphs.Market
{
    internal class EVPerformancePrepared : EVYearsPrepared
    {
        internal IDictionary<int, decimal?> AveragePerformanceByYear { get; }

        internal EVPerformancePrepared(IEnumerable<int> yearsSorted, IDictionary<int, decimal?> averagePerformancePerYear)
            : base(yearsSorted)
        {
            this.AveragePerformanceByYear = averagePerformancePerYear;
        }
    }

    internal class EVPerformanceGraph : YearsGraph<EVPerformancePrepared>
    {
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

        private static IGraphModelType<IEnumerable<Vehicle>, EVPerformancePrepared> MODEL
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

        internal static readonly EVPerformanceGraph INSTANCE = new EVPerformanceGraph();

        private EVPerformanceGraph()
            : base(MODEL)
        {

        }
    }
}

