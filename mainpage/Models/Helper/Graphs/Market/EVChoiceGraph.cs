using System;

using System.Collections.Generic;
using System.Linq;

using CleanTechSim.MainPage.Models.Domain;
using CleanTechSim.MainPage.Models.Helper.GraphData;
using CleanTechSim.MainPage.Models.Helper.GraphData.Prepare;

namespace CleanTechSim.MainPage.Models.Helper.Graphs.Market
{

    internal class EVChoicePrepared : EVYearsPrepared
    {
        internal IDictionary<int, int> NumberOfNewModelsByYear { get; }

        internal EVChoicePrepared(IEnumerable<int> yearsSorted, IDictionary<int, int> numberOfModelsByYear)
            : base(yearsSorted)
        {
            this.NumberOfNewModelsByYear = numberOfModelsByYear;
        }
    }

    internal class EVChoiceGraph : YearsGraph
    {
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

        internal static IGraphModelType<IEnumerable<Vehicle>, EVChoicePrepared> MODEL
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
    }
}
