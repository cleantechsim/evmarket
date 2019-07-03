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

        private readonly IDataStorage storage;

        public HomeController()
        {
            string connectionString = Environment.GetEnvironmentVariable("AZURE_TABLE_STORAGE_CONNECTION_STRING");

            this.storage = new AzureTableStorage(connectionString, typeof(MonthlyCountryEVCarSales));
        }

        private LineGraph GetAll<T, KEY>(Type type, IGraphModelType<T, KEY> graphModelType)
        {
            IEnumerable<T> elements = storage.GetAll<T>(type);

            IDictionary<KEY, List<T>> dictionary = graphModelType.GetByDistinctKeys(elements);

            // Add datapoints


            List<LineInfo> lineInfos = new List<LineInfo>(dictionary.Keys.Count());

            foreach (KEY key in dictionary.Keys)
            {
                List<T> lineElements = dictionary[key];

                List<DataPoint> dataPoints = new List<DataPoint>(lineElements.Count());

                foreach (T lineElement in lineElements)
                {
                    DataPoint dataPoint = new DataPoint(
                        graphModelType.GetDataPointX(lineElement),
                        graphModelType.GetDataPointY(lineElement),
                        graphModelType.GetSources(lineElement)
                    );

                    dataPoints.Add(dataPoint);
                }

                DataSeries dataSeries = new DataSeries(null, dataPoints);

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
                    GetAll(typeof(MonthlyCountryEVCarSales), StaticData.EvAdoptionGraph)));

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
