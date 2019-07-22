
using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using CleanTechSim.MainPage.Models;
using CleanTechSim.MainPage.Models.Domain;

using CleanTechSim.MainPage.Helpers.Storage.AzureTableStorage;
using CleanTechSim.MainPage.Models.Helper.ClientGraph;
using CleanTechSim.MainPage.Models.Helper.GraphData;
using CleanTechSim.MainPage.Models.Helper.Graphs;

namespace CleanTechSim.MainPage.Controllers
{
    public abstract class BaseController : Controller
    {
        private AzureTableStorage storage;

        public BaseController()
        {
            string connectionString = Environment.GetEnvironmentVariable("AZURE_TABLE_STORAGE_CONNECTION_STRING");

            this.storage = new AzureTableStorage(
                connectionString,
                typeof(MonthlyCountryEVCarSales),
                typeof(BatteryCost),
                typeof(Vehicle));
        }

        internal IEnumerable<T> GetAll<T>(Type type)
        {
            return storage.GetAll<T>(type);
        }

        internal LineGraph GetAllMultiLine<INSTANCE, PREPARED>(Type type, InstanceStatsGraph<INSTANCE, PREPARED> graph)
        {
            IEnumerable<INSTANCE> elements = GetAll<INSTANCE>(type);

            return graph.GetAllMultiLine(elements);
        }

        internal LineGraph GetAllSingleLine<INSTANCE, PREPARED>(Type type, InstanceStatsGraph<INSTANCE, PREPARED> graph)
        {
            IEnumerable<INSTANCE> elements = GetAll<INSTANCE>(type);

            return graph.GetAllSingleLine(elements);
        }

        internal LineGraph GetAllMultiLineKeyed<T, KEY>(Type type, KeyedInstanceStatsGraph<T, KEY> graph)
        {
            IEnumerable<T> elements = GetAll<T>(type);

            return graph.GetAllMultiLineKeyed(elements);
        }

        internal static StatsGraphModel VerifyAndComputeStatsModel(string graphId, LineGraph lineGraph)
        {
            PreparedDataPoints dataPoints = PreparedDataPoints.VerifyAndCompute(lineGraph);

            return new StatsGraphModel(graphId, lineGraph.Title, lineGraph.SubTitle, dataPoints);
        }
    }
}
