using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CleanTechSim.MainPage.Models;
using CleanTechSim.MainPage.Models.Persistent;
using CleanTechSim.MainPage.Helpers;
using CleanTechSim.MainPage.Helpers.Model;
using CleanTechSim.MainPage.Helpers.Storage;
using CleanTechSim.MainPage.Helpers.Storage.AzureTableStorage;

namespace mainpage.Controllers
{
    public class HomeController : Controller
    {
        public const string EV_ADOPTION_ID = "evAdoption";
        public const string BATTERY_COST_ID = "batteryCost";
        public const string EV_RANGE_ID = "evRange";
        public const string EV_CHOICE_ID = "evChoice";

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

        private DataSeries GetDataSeries<T, PREPARED>(IEnumerable<T> lineElements, IGraphModelType<IEnumerable<T>, PREPARED> graphModelType)
        {

            PREPARED prepared = graphModelType.Prepare(lineElements);

            int num = graphModelType.GetNumX(lineElements, prepared);
            List<DataPoint> dataPoints = new List<DataPoint>(num);

            for (int i = 0; i < num; ++i)
            {
                T lineElement = lineElements.ElementAt(i);

                DataPoint dataPoint = new DataPoint(
                    graphModelType.GetDataPointX(lineElements, prepared, i),
                    graphModelType.GetDataPointY(lineElements, prepared, i),
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

            DataSeries dataSeries = GetDataSeries<INSTANCE, PREPARED>(elements, graphModelType);

            return new LineGraph(
                graphModelType.Title,
                graphModelType.SubTitle,
                graphModelType.DataPointFormat,
                new Line[] { new Line(null, Color.Green, dataSeries) });
        }

        private LineGraph GetAllMultiLine<T, KEY>(Type type, IMultiLineGraphModelType<T, KEY> graphModelType)
        {
            IEnumerable<T> elements = storage.GetAll<T>(type);

            IDictionary<KEY, List<T>> dictionary = graphModelType.GetByDistinctKeys(elements);

            // Add datapoints
            List<LineInfo> lineInfos = new List<LineInfo>(dictionary.Keys.Count());

            foreach (KEY key in dictionary.Keys)
            {
                DataSeries dataSeries = GetDataSeries(dictionary[key], graphModelType);

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
                    Color.Colors.ElementAt(i),
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

                this.dataSeriesSum = dataSeries.DataPoints.Sum(dataPoint => dataPoint.YValue);
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
                PreparedDataPoints.VerifyAndCompute(
                    EV_ADOPTION_ID,
                    GetAllMultiLine(typeof(MonthlyCountryEVCarSales), StaticData.EvAdoptionGraph)),

                PreparedDataPoints.VerifyAndCompute(
                    BATTERY_COST_ID,
                    GetAllSingleLine(typeof(BatteryCost), StaticData.BatteryCostGraph)),

                PreparedDataPoints.VerifyAndCompute(
                    EV_RANGE_ID,
                    GetAllSingleLine(typeof(Vehicle), StaticData.EVRangeGraph)),

                PreparedDataPoints.VerifyAndCompute(
                    EV_CHOICE_ID,
                    GetAllSingleLine(typeof(Vehicle), StaticData.EVChoiceGraph))
            );

            return View(model);
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
