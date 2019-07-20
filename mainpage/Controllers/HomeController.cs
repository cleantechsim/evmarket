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

using CleanTechSim.MainPage.Helpers;

namespace CleanTechSim.MainPage.Controllers
{
    public class HomeController : Controller
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

        private DataSeries GetDataSeries<T, PREPARED>(IEnumerable<T> lineElements, IGraphModelType<IEnumerable<T>, PREPARED> graphModelType, int lineNo)
        {

            PREPARED prepared = graphModelType.Prepare(lineElements);

            int num = graphModelType.GetNumX(lineElements, prepared);
            List<DataPoint> dataPoints = new List<DataPoint>(num);

            for (int i = 0; i < num; ++i)
            {
                T lineElement = lineElements.ElementAt(i);

                DataPoint dataPoint = new DataPoint(
                    graphModelType.GetDataPointX(lineElements, prepared, lineNo, i),
                    graphModelType.GetDataPointY(lineElements, prepared, lineNo, i),
                    graphModelType.GetSources(lineElements, prepared, i)
                );

                dataPoints.Add(dataPoint);
            }

            DataSeries dataSeries = new DataSeries(null, dataPoints);

            return dataSeries;
        }

        private LineGraph GetAllSingleLine<INSTANCE, PREPARED>(Type type, IGraphModelType<IEnumerable<INSTANCE>, PREPARED> graphModelType)
        {
            IEnumerable<INSTANCE> elements = storage.GetAll<INSTANCE>(type);

            DataSeries dataSeries = GetDataSeries<INSTANCE, PREPARED>(elements, graphModelType, 0);

            return new LineGraph(
                graphModelType.Title,
                graphModelType.SubTitle,
                graphModelType.DataPointFormat,
                new Line[] { new Line(null, Color.Green, dataSeries) });
        }

        private LineGraph GetAllMultiLine<INSTANCE, PREPARED>(Type type, IGraphModelType<IEnumerable<INSTANCE>, PREPARED> graphModelType)
        {
            IEnumerable<INSTANCE> elements = storage.GetAll<INSTANCE>(type);

            Line[] lines = new Line[graphModelType.NumLines];

            for (int lineNo = 0; lineNo < graphModelType.NumLines; ++lineNo)
            {
                DataSeries dataSeries = GetDataSeries<INSTANCE, PREPARED>(elements, graphModelType, lineNo);

                string lineLabel = graphModelType.GetLineLabel(lineNo);

                lines[lineNo] = new Line(lineLabel, ColorForLine(lineNo), dataSeries);
            }

            return new LineGraph(
                graphModelType.Title,
                graphModelType.SubTitle,
                graphModelType.DataPointFormat,
                lines);
        }

        private static Color ColorForLine(int lineNo)
        {
            return Color.Colors.ElementAt(lineNo);
        }

        private LineGraph GetAllMultiLine<T, KEY>(Type type, IMultiLineGraphModelType<T, KEY> graphModelType)
        {
            IEnumerable<T> elements = storage.GetAll<T>(type);

            IDictionary<KEY, List<T>> dictionary = graphModelType.GetByDistinctKeys(elements);

            // Add datapoints
            List<LineInfo> lineInfos = new List<LineInfo>(dictionary.Keys.Count());

            foreach (KEY key in dictionary.Keys)
            {
                DataSeries dataSeries = GetDataSeries(dictionary[key], graphModelType, lineInfos.Count());

                LineInfo lineInfo = new LineInfo(graphModelType.GetLineLabel(key), dataSeries);

                lineInfos.Add(lineInfo);
            }

            // Sort in descending order
            lineInfos.Sort((value1, value2) => -value1.CompareTo(value2));

            int num = lineInfos.Count();

            List<Line> lines = new List<Line>(num);

            for (int i = 0; i < num; ++i)
            {
                LineInfo lineInfo = lineInfos.ElementAt(i);
                Line line = new Line(
                    lineInfo.Label,
                    ColorForLine(i),
                    lineInfo.DataSeries
                );

                lines.Add(line);
            }

            return new LineGraph(
                graphModelType.Title,
                graphModelType.SubTitle,
                graphModelType.DataPointFormat,
                lines);
        }

        internal class LineInfo : IComparable
        {
            public string Label { get; }
            public DataSeries DataSeries { get; }

            private decimal dataSeriesSum;

            public LineInfo(string label, DataSeries dataSeries)
            {
                this.Label = label;
                this.DataSeries = dataSeries;

                this.dataSeriesSum = dataSeries.DataPoints.Sum(dataPoint => dataPoint.YValue.HasValue ? dataPoint.YValue.Value : 0m);
            }

            public int CompareTo(object obj)
            {
                LineInfo other = (LineInfo)obj;

                return this.dataSeriesSum.CompareTo(other.dataSeriesSum);
            }
        }
        public IActionResult Index()
        {
            IndexModel model = new IndexModel(
                VerifyAndComputeStaticModel(
                    GraphIds.EV_ADOPTION_ID,
                    GetAllMultiLine(typeof(MonthlyCountryEVCarSales), StaticData.EvAdoptionGraph)),

                VerifyAndComputeStaticModel(
                    GraphIds.BATTERY_COST_ID,
                    GetAllSingleLine(typeof(BatteryCost), StaticData.BatteryCostGraph)),

                VerifyAndComputeStaticModel(
                    GraphIds.EV_RANGE_ID,
                    GetAllMultiLine(typeof(Vehicle), StaticData.EVRangeGraph)),

                VerifyAndComputeStaticModel(
                    GraphIds.EV_CHOICE_ID,
                    GetAllSingleLine(typeof(Vehicle), StaticData.EVChoiceGraph)),

                VerifyAndComputeStaticModel(
                    GraphIds.EV_PERFORMANCE_ID,
                    GetAllSingleLine(typeof(Vehicle), StaticData.EVPerformanceGraph)),

                VerifyAndComputeStaticModel(
                    GraphIds.EV_SALES_PRICE_ID,
                    GetAllMultiLine(typeof(Vehicle), StaticData.EVSalesPriceGraph)),

                MakeDynamicModel(
                    GraphIds.INCOME_ID,
                    DynamicData.IncomeGraph
                ),
                MakeDynamicModel(
                    GraphIds.RANGE_REQUIREMENT_ID,
                    DynamicData.RangeRequirementGraph
                ),
                MakeDynamicModel(
                    GraphIds.PROPENSITY_ID,
                    DynamicData.PropensityGraph
                )
            );

            return View(model);
        }

        private static StaticGraphModel VerifyAndComputeStaticModel(string graphId, LineGraph lineGraph)
        {
            PreparedDataPoints dataPoints = PreparedDataPoints.VerifyAndCompute(lineGraph);

            return new StaticGraphModel(graphId, lineGraph.Title, lineGraph.SubTitle, dataPoints);
        }

        private static DynamicGraphModel MakeDynamicModel(
            string graphId,
            DynamicGraph graph)
        {
            return new DynamicGraphModel(
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
