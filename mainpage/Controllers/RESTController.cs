
using System;

using CleanTechSim.MainPage.Helpers;

using CleanTechSim.MainPage.Models.Domain;

using CleanTechSim.MainPage.Models.Helper.ClientGraph;
using CleanTechSim.MainPage.Models.Helper.GraphData;

using CleanTechSim.MainPage.Models.Helper.Graphs;
using CleanTechSim.MainPage.Models.Helper.Graphs.Market;
using CleanTechSim.MainPage.Models.Helper.Graphs.Consumer;

using System.Collections.Generic;

namespace CleanTechSim.MainPage.Controllers
{
    public class RESTController : BaseController
    {
        // For input graphs
        public IDictionary<string, object> GetData(string graphId, decimal median, decimal dispersion, decimal skew)
        {

            if (dispersion <= 0m)
            {
                // avoid divide by zero
                dispersion = 0.00000001m;
            }

            InputGraph graph;

            // Get from static data
            switch (graphId)
            {
                case GraphIds.INCOME_ID:
                    graph = IncomeGraph.INSTANCE;
                    break;

                case GraphIds.RANGE_REQUIREMENT_ID:
                    graph = RangeRequirementsGraph.INSTANCE;
                    break;

                case GraphIds.PROPENSITY_ID:
                    graph = EVPurchasePropensityGraph.INSTANCE;
                    break;

                default:
                    throw new NotImplementedException();
            }

            PreparedDataPoints dataPoints = graph.GenerateDataPoints(median, dispersion, skew, graph.MaxXValue);

            return MakeGraphJSONDictionary(dataPoints, graph.SuggestedMaxY);
        }

        // For compute market share graph
        public IDictionary<string, object> ComputeMarketForecast(
            string graphId,
            decimal incomeMedian,
            decimal incomeDispersion,
            decimal incomeSkew,
            decimal percentageOfWageForCarCost)
        {

            ForecastInput input = new ForecastInput(
                GetAll<Vehicle>(typeof(Vehicle)),
                new InputGraphSelection(incomeMedian, incomeDispersion, incomeSkew),
                percentageOfWageForCarCost);

            LineGraph lineGraph = EVMarketForecastGraph.INSTANCE.GetAllSingleLine(input);

            PreparedDataPoints dataPoints = PreparedDataPoints.VerifyAndCompute(lineGraph);

            return MakeGraphJSONDictionary(dataPoints, null);
        }

        private static IDictionary<string, object> MakeGraphJSONDictionary(PreparedDataPoints dataPoints, decimal? suggestedMaxY)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            dictionary.Add("labels", dataPoints.Labels);

            List<IDictionary<string, object>> dataSets = new List<IDictionary<string, object>>();

            foreach (DataSet dataSet in dataPoints.DataSets)
            {
                IDictionary<string, object> dataSetDictionay = new Dictionary<string, object>();

                dataSetDictionay.Add("data", dataSet.Values);

                dataSetDictionay.Add("pointRadius", 0);

                dataSets.Add(dataSetDictionay);
            }

            dictionary.Add("datasets", dataSets);

            if (suggestedMaxY.HasValue)
            {
                dictionary.Add("suggestedMaxY", suggestedMaxY.Value);
            }

            return dictionary;
        }
    }
}

