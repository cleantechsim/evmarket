
using System;
using System.Collections.Generic;
using System.Linq;

using CleanTechSim.MainPage.Models;
using CleanTechSim.MainPage.Models.Persistent;
using CleanTechSim.MainPage.Helpers.Model;

namespace CleanTechSim.MainPage.Helpers
{
    public class StaticData
    {
        public static IMultiLineGraphModelType<MonthlyCountryEVCarSales, string> EvAdoptionGraph = new MultiLineGraphModelType<MonthlyCountryEVCarSales, string>(

            "EV percent marketshare by month",

            Encoding.YEAR_MONTH,
            Encoding.NUMBER,

            instance => instance.Year + ((decimal)instance.Month) / 100,
            instance => instance.SalesPercent,

            instance => instance.Country,
            key => key
        );

        public static ISingleLineGraphModelType<BatteryCost> BatteryCostGraph = new SingleLineGraphModelType<BatteryCost>(
            "Battery cost in $/kWh",

            Encoding.YEAR,
            Encoding.NUMBER,

            instance => instance.Century * 100 + instance.Year,
            instance => instance.Cost
        );


        private static List<int> GetDistinctSortedYearsForVehicles(IEnumerable<Vehicle> instances)
        {
            HashSet<int> distinctYears = new HashSet<int>();

            foreach (Vehicle v in instances)
            {
                if (v.Year.HasValue)
                {
                    distinctYears.Add(v.Year.Value);
                }
            }

            List<int> sortedYears = new List<int>(distinctYears);

            sortedYears.Sort();

            return sortedYears;
        }


        public class EVRangePrepared
        {
            internal IEnumerable<int> YearsSorted { get; }
            internal IDictionary<int, int> AverageRangeByYear { get; }

            internal EVRangePrepared(IEnumerable<int> yearsSorted, IDictionary<int, int> averageRangePerYear)
            {
                this.YearsSorted = yearsSorted;
                this.AverageRangeByYear = averageRangePerYear;
            }
        }

        internal static EVRangePrepared ComputeAverageRangePerYear(IEnumerable<Vehicle> instances)
        {
            Dictionary<Vehicle, int> wltpForCar = Vehicle.ComputeVehicleToWLTPRange(instances);
            List<int> sortedYears = GetDistinctSortedYearsForVehicles(instances);

            // Make dictionary of average by year
            Dictionary<int, int> averageRangeByYear = new Dictionary<int, int>();

            foreach (int year in sortedYears)
            {
                IEnumerable<Vehicle> yearVehicles = from Vehicle v in instances
                                                    where
                                                           v.Year != null
                                                        && v.Year.Value == year
                                                    select v;

                Console.WriteLine("## vehicles for year " + yearVehicles.Count());

                averageRangeByYear[year] = (int)(from Vehicle v in yearVehicles
                                                 where wltpForCar.ContainsKey(v)
                                                 select wltpForCar[v]).Average();


            }
            return new EVRangePrepared(sortedYears, averageRangeByYear);
        }

        public static IGraphModelType<IEnumerable<Vehicle>, EVRangePrepared> EVRangeGraph
            = new GraphModelType<IEnumerable<Vehicle>, EVRangePrepared>(
            "EV average range for new models",

            Encoding.YEAR,
            Encoding.NUMBER,

            instances => ComputeAverageRangePerYear(instances),

            (instances, prepared) => prepared.YearsSorted.Count(),

            (instances, prepared, index) => prepared.YearsSorted.ElementAt(index),
            (instances, prepared, index) => prepared.AverageRangeByYear[prepared.YearsSorted.ElementAt(index)]
        );

    }
}
