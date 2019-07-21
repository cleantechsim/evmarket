
using System.Collections.Generic;
using System.Linq;

using CleanTechSim.MainPage.Helpers;

using CleanTechSim.MainPage.Models.Domain;
using CleanTechSim.MainPage.Models.Helper.GraphData;
using CleanTechSim.MainPage.Models.Helper.GraphData.Prepare;

namespace CleanTechSim.MainPage.Models.Helper.Graphs.Market
{
    internal class EVRangeGraph : YearsGraph
    {
        internal class EVRangePrepared : EVYearsPrepared
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

        public static IGraphModelType<IEnumerable<Vehicle>, EVRangePrepared> MODEL
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
    }
}
