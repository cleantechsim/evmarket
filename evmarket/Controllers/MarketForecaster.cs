
using System;

using System.Collections.Generic;

namespace CleanTechSim.EVMarket.Helpers.Compute
{
    public class MarketForecaster
    {

        // Generate series of montly values
        public static IEnumerable<decimal> ProduceYearlyForecast(

            int numberOfYears,

            // For now just take average of median car cost to figure out car cost
            decimal averageOfYearlyMedianCarCost,

            decimal percentageOfWageForCarCost,

            // How large percentage of consumers above cost threshold?
            Func<decimal, decimal> probabilityOfConsumersAboveIncomeThreshold,

            // How large is percentage that would prefer EVSs, else being equal
            decimal percentageOfConsumersWithEVPropensity,

            // Percentage of yearly car cost reduction due to economies of scale
            decimal yearlyCostReductionPercent
        )
        {
            decimal[] values = new decimal[numberOfYears];

            decimal curCost = averageOfYearlyMedianCarCost;

            for (int yearNo = 0; yearNo < numberOfYears; ++yearNo)
            {
                // Console.WriteLine("## cost for year " + yearNo);

                // Figure how much can spend for car
                decimal wageRequiredForBuyingCar = (curCost * 100m) / percentageOfWageForCarCost;

                // Console.WriteLine("## can spend " + wageRequiredForBuyingCar + " on car");

                decimal shareAboveWage = probabilityOfConsumersAboveIncomeThreshold.Invoke(wageRequiredForBuyingCar);

                decimal percentAboveWage = shareAboveWage * 100m;

                decimal marketShare = (shareAboveWage / 100m) * (percentageOfConsumersWithEVPropensity / 100) * 100m;

                values[yearNo] = marketShare;

                curCost -= (curCost * yearlyCostReductionPercent) / 100m;
            }

            return values;
        }
    }
}

