
using System.Collections.Generic;
using NUnit.Framework;

using CleanTechSim.MainPage.Models.Helper.ClientGraph;
using CleanTechSim.MainPage.Models.Helper.GraphData;
using CleanTechSim.MainPage.Helpers;

namespace CleanTechSim.MainPage.Models
{
    public class PreparedDataPointsTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestPrepareSameLength()
        {
            List<Line> lines = new List<Line>();

            Line norway = new Line(
                "Norway",
                Color.Green,

                new DataSeries(
                    new DataPoint(2019.01m, 3.8m, "https://gronnkontakt.no/2019/02/elbilsalget-i-januar"),
                    new DataPoint(2019.02m, 40.7m, "https://www.elbil24.no/nyheter/her-er-vinnerne-og-taperne/70831731"),
                    new DataPoint(2019.03m, 58.4m,
                        "https://elbil.no/norway-reaches-historic-electric-car-market-share",
                        "https://www.electrive.com/2019/04/02/58-4-of-all-new-vehicles-in-norway-are-purely-electric")
            ));

            lines.Add(norway);

            Line china = new Line(
                "China",
                Color.Blue,

                new DataSeries(
                    new DataPoint(2019.01m, 4.8m, "https://insideevs.com/news/343009/in-january-plug-in-ev-car-sales-in-china-almost-tripled"),
                    new DataPoint(2019.02m, 4.3m, "https://insideevs.com/news/343600/plug-in-electric-car-sales-increased-in-china-in-february"),
                    new DataPoint(2019.03m, 5.8m, "https://insideevs.com/news/346882/march-2019-sales-china")
            ));

            lines.Add(china);


            PreparedDataPoints dataPoints = PreparedDataPoints.VerifyAndCompute(
                new LineGraph(
                    "Test lines",
                    "Subtitle",
                    new DataPointFormat(Encoding.YEAR_MONTH, Encoding.NUMBER),
                    lines
                )
            );

            Assert.IsNotNull(dataPoints);
        }
    }
}