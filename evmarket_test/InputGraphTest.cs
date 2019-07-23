
using NUnit.Framework;
using System.Linq;

namespace CleanTechSim.EVMarket.Models.Helper.Graphs
{


    public class InputGraphTest
    {
        [Test]
        public void TestDigitsToArray()
        {
            Assert.AreEqual(new int[] { 1, 2, 3 }, InputGraph.ToDigitsArray(123).ToArray());
            Assert.AreEqual(new int[] { }, InputGraph.ToDigitsArray(0).ToArray());
            Assert.AreEqual(new int[] { 1, 0, 0 }, InputGraph.ToDigitsArray(100).ToArray());
            Assert.AreEqual(new int[] { 4, 7, 3 }, InputGraph.ToDigitsArray(473).ToArray());
        }


        [Test]
        public void TestMakeDecimal()
        {
            Assert.AreEqual(100, InputGraph.MakeDecimal(1, 2));
            Assert.AreEqual(0, InputGraph.MakeDecimal(0, 0));
            Assert.AreEqual(50, InputGraph.MakeDecimal(5, 1));
            Assert.AreEqual(5, InputGraph.MakeDecimal(5, 0));
        }

        [Test]
        public void TestFindNearestRounding()
        {
            Assert.AreEqual(10, InputGraph.FindNearestRounding(10m));

            Assert.AreEqual(500, InputGraph.FindNearestRounding(473.2m));
            Assert.AreEqual(500, InputGraph.FindNearestRounding(500.0m));

            Assert.AreEqual(1000, InputGraph.FindNearestRounding(500.1m));
            Assert.AreEqual(1000, InputGraph.FindNearestRounding(963.4m));
            Assert.AreEqual(5000, InputGraph.FindNearestRounding(1000.1m));
            Assert.AreEqual(5000, InputGraph.FindNearestRounding(1001m));
        }

        [Test]
        public void TestFindGraphPercentageIntervals()
        {
            int numIntervals;

            Assert.AreEqual(5000, InputGraph.FindGraphPercentageIntervals(25000m, 50000m, 10, out numIntervals));

            Assert.AreEqual(10, numIntervals);

            Assert.AreEqual(10, InputGraph.FindGraphPercentageIntervals(50m, 100m, 10, out numIntervals));
            Assert.AreEqual(10, numIntervals);
        }
    }

}

