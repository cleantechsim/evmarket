using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using CleanTechSim.EVMarket.Models;
using CleanTechSim.EVMarket.Models.Domain;
using CleanTechSim.EVMarket.Models.Helper.GraphData.Prepare;
using CleanTechSim.EVMarket.Helpers.Storage;
using CleanTechSim.EVMarket.Helpers.Storage.AzureTableStorage;

using CleanTechSim.EVMarket.Models.Helper.ClientGraph;
using CleanTechSim.EVMarket.Models.Helper.GraphData;
using CleanTechSim.EVMarket.Models.Helper.Graphs;
using CleanTechSim.EVMarket.Models.Helper.Graphs.Consumer;
using CleanTechSim.EVMarket.Models.Helper.Graphs.Market;
using CleanTechSim.EVMarket.Helpers;

namespace CleanTechSim.EVMarket.Controllers
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

        public IActionResult Index()
        {
            IEnumerable<string> proxiedUriHeaderValue = Request.Headers["X-Proxied-URI"];
            string proxiedUri = proxiedUriHeaderValue.Any() ? proxiedUriHeaderValue.ElementAt(0) : "";

            ViewData["files_prefix"] = proxiedUri;

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

                MakeComputeGraphModel(GraphIds.MARKET_FORECAST, EVMarketForecastGraph.INSTANCE, proxiedUri, "computeMarketForecast"),

                MakeInputGraphModel(
                    GraphIds.INCOME_ID,
                    IncomeGraph.INSTANCE,
                    proxiedUri
                ),
                MakeInputGraphModel(
                    GraphIds.RANGE_REQUIREMENT_ID,
                    RangeRequirementsGraph.INSTANCE,
                    proxiedUri
                ),
                MakeInputGraphModel(
                    GraphIds.PROPENSITY_ID,
                    EVPurchasePropensityGraph.INSTANCE,
                    proxiedUri
                )
            );

            return View(model);
        }

        private static InputGraphModel MakeInputGraphModel(
            string graphId,
            InputGraph graph,
            string proxiedUri)
        {
            return new InputGraphModel(
                graphId,
                graph.Title,
                graph.SubTitle,
                proxiedUri,
                "/REST/getData",
                graph.Median,
                graph.Dispersion,
                graph.Skew);
        }

        private static ComputeGraphModel MakeComputeGraphModel(string graphId, StatsGraph graph, string proxiedUri, string url)
        {
            return new ComputeGraphModel(
                graphId,
                graph.Title,
                graph.SubTitle,
                proxiedUri,
                "/REST/" + url);
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
