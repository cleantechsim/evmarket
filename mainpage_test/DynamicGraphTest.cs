
using NUnit.Framework;
using System.Linq;

namespace CleanTechSim.MainPage.Models.Helper.GraphData.Prepare
{


    public class DynamicGraphTest
    {
        [Test]
        public void TestDigitsToArray()
        {
            Assert.AreEqual(new int[] { 1, 2, 3 }, DynamicGraph.ToDigitsArray(123).ToArray());
            Assert.AreEqual(new int[] { }, DynamicGraph.ToDigitsArray(0).ToArray());
            Assert.AreEqual(new int[] { 1, 0, 0 }, DynamicGraph.ToDigitsArray(100).ToArray());
            Assert.AreEqual(new int[] { 4, 7, 3 }, DynamicGraph.ToDigitsArray(473).ToArray());
        }


        [Test]
        public void TestMakeDecimal()
        {
            Assert.AreEqual(100, DynamicGraph.MakeDecimal(1, 2));
            Assert.AreEqual(0, DynamicGraph.MakeDecimal(0, 0));
            Assert.AreEqual(50, DynamicGraph.MakeDecimal(5, 1));
            Assert.AreEqual(5, DynamicGraph.MakeDecimal(5, 0));
        }

        [Test]
        public void TestFindNearestRounding()
        {
            Assert.AreEqual(500, DynamicGraph.FindNearestRounding(473.2m));
            Assert.AreEqual(500, DynamicGraph.FindNearestRounding(500.0m));

            Assert.AreEqual(1000, DynamicGraph.FindNearestRounding(500.1m));
            Assert.AreEqual(1000, DynamicGraph.FindNearestRounding(963.4m));
            Assert.AreEqual(5000, DynamicGraph.FindNearestRounding(1000.1m));
            Assert.AreEqual(5000, DynamicGraph.FindNearestRounding(1001m));
        }

        [Test]
        public void TestFindGraphPercentageIntervals()
        {
            int numIntervals;

            Assert.AreEqual(5000, DynamicGraph.FindGraphPercentageIntervals(25000m, 50000m, 10, out numIntervals));

            Assert.AreEqual(10, numIntervals);
        }
    }

}

