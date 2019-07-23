
using System.Collections.Generic;

using CleanTechSim.EVMarket.Models.Domain;

using CleanTechSim.EVMarket.Models.Helper.GraphData.Prepare;

namespace CleanTechSim.EVMarket.Models.Helper.Graphs.Market
{

    internal class EVYearsPrepared
    {
        internal IEnumerable<int> YearsSorted { get; }

        internal EVYearsPrepared(IEnumerable<int> yearsSorted)
        {
            this.YearsSorted = yearsSorted;
        }
    }

    internal class YearsGraph<PREPARED> : InstanceStatsGraph<Vehicle, PREPARED> where PREPARED : EVYearsPrepared
    {
        internal YearsGraph(IGraphModelType<IEnumerable<Vehicle>, PREPARED> graphModelType)
            : base(graphModelType)
        {

        }

        internal static List<int> GetDistinctSortedYearsForVehicles(IEnumerable<Vehicle> instances, int? maxYear = null)
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
    }

}