using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using CleanTechSim.MainPage.Models;
using CleanTechSim.MainPage.Models.Domain;
using CleanTechSim.MainPage.Models.Helper.GraphData.Prepare;
using CleanTechSim.MainPage.Helpers.Storage;
using CleanTechSim.MainPage.Helpers.Storage.AzureTableStorage;

using CleanTechSim.MainPage.Models.Helper.ClientGraph;
using CleanTechSim.MainPage.Models.Helper.GraphData;
using CleanTechSim.MainPage.Models.Helper.Graphs;
using CleanTechSim.MainPage.Models.Helper.Graphs.Consumer;
using CleanTechSim.MainPage.Models.Helper.Graphs.Market;
using CleanTechSim.MainPage.Helpers;


namespace CleanTechSim.MainPage.Controllers
{
    public class HomeController : BaseController
    {

        private readonly IDataStorage storage;

        public HomeController()
        {
            string connectionString = Environment.GetEnvironmentVariable("AZURE_TABLE_STORAGE_CONNECTION_STRING");

            this.storage = new AzureTableStorage(
                connectionString,
                typeof(MonthlyCountryEVCarSales),
                typeof(BatteryCost),
                typeof(Vehicle));
        }

        private LineGraph GetAllMultiLine<INSTANCE, PREPARED>(Type type, InstanceStatsGraph<INSTANCE, PREPARED> graph)
        {
            IEnumerable<INSTANCE> elements = GetAll<INSTANCE>(type);

            return graph.GetAllMultiLine(elements);
        }

        private LineGraph GetAllSingleLine<INSTANCE, PREPARED>(Type type, InstanceStatsGraph<INSTANCE, PREPARED> graph)
        {
            IEnumerable<INSTANCE> elements = GetAll<INSTANCE>(type);

            return graph.GetAllSingleLine(elements);
        }

        private LineGraph GetAllMultiLineKeyed<T, KEY>(Type type, KeyedInstanceStatsGraph<T, KEY> graph)
        {
            IEnumerable<T> elements = GetAll<T>(type);

            return graph.GetAllMultiLineKeyed(elements);
        }

        public IActionResult Index()
        {
            IndexModel model = new IndexModel(
                VerifyAndComputeStatsModel(
                    GraphIds.EV_ADOPTION_ID,
                    GetAllMultiLineKeyed(typeof(MonthlyCountryEVCarSales), EVAdoptionGraph.INSTANCE)),

                VerifyAndComputeStatsModel(
                    GraphIds.BATTERY_COST_ID,
                    GetAllSingleLine(typeof(BatteryCost), BatteryCostGraph.INSTANCE)),

                VerifyAndComputeStatsModel(
                    GraphIds.EV_RANGE_ID,
                    GetAllMultiLine(typeof(Vehicle), EVRangeGraph.INSTANCE)),

                VerifyAndComputeStatsModel(
                    GraphIds.EV_CHOICE_ID,
                    GetAllSingleLine(typeof(Vehicle), EVChoiceGraph.INSTANCE)),

                VerifyAndComputeStatsModel(
                    GraphIds.EV_PERFORMANCE_ID,
                    GetAllSingleLine(typeof(Vehicle), EVPerformanceGraph.INSTANCE)),

                VerifyAndComputeStatsModel(
                    GraphIds.EV_SALES_PRICE_ID,
                    GetAllMultiLine(typeof(Vehicle), EVSalesPriceGraph.INSTANCE)),

                MakeInputGraphModel(
                    GraphIds.INCOME_ID,
                    IncomeGraph.INSTANCE
                ),
                MakeInputGraphModel(
                    GraphIds.RANGE_REQUIREMENT_ID,
                    RangeRequirementsGraph.INSTANCE
                ),
                MakeInputGraphModel(
                    GraphIds.PROPENSITY_ID,
                    EVPurchasePropensityGraph.INSTANCE
                )
            );

            return View(model);
        }

        private static StatsGraphModel VerifyAndComputeStatsModel(string graphId, LineGraph lineGraph)
        {
            PreparedDataPoints dataPoints = PreparedDataPoints.VerifyAndCompute(lineGraph);

            return new StatsGraphModel(graphId, lineGraph.Title, lineGraph.SubTitle, dataPoints);
        }

        private static InputGraphModel MakeInputGraphModel(
            string graphId,
            InputGraph graph)
        {
            return new InputGraphModel(
                graphId,
                graph.Title,
                graph.SubTitle,
                "/REST/getData",
                graph.Median,
                graph.Dispersion,
                graph.Skew);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
