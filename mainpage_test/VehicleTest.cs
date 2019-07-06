
using System.Collections.Generic;

using NUnit.Framework;

namespace CleanTechSim.MainPage.Models.Persistent
{

    public class VehicleTest
    {
        private static Vehicle MakeVehicle(
            string make,
            string model,
            int? rangeEPA,
            int? rangeWLTP,
            int? rangeNEDC)
        {
            Vehicle vehicle = new Vehicle();

            vehicle.Make = make;
            vehicle.Model = model;

            vehicle.RangeEPA = rangeEPA;
            vehicle.RangeWLTP = rangeWLTP;
            vehicle.RangeNEDC = rangeNEDC;

            return vehicle;
        }

        [Test]
        public void TestGetEstimatedWLTPRangeFromEPA()
        {
            Vehicle vehicle1 = MakeVehicle("Make", "Model", 100, 150, null);
            Vehicle vehicle2 = MakeVehicle("AnotherMake", "AnotherModel", 120, 240, null);

            Vehicle epaOnly = MakeVehicle("EPAMake", "EPAModel", 200, null, null);

            // EPA to WLTP factor is (150/100 + 200/150) / 2
            decimal epaToWLTPFactor1 = vehicle1.RangeWLTP.Value / (decimal)vehicle1.RangeEPA.Value;
            Assert.AreEqual(epaToWLTPFactor1, 1.5m);
            Assert.AreEqual(vehicle1.RangeWLTP, vehicle1.RangeEPA * epaToWLTPFactor1);

            decimal epaToWLTPFactor2 = vehicle2.RangeWLTP.Value / (decimal)vehicle2.RangeEPA.Value;
            Assert.AreEqual(epaToWLTPFactor2, 2.0m);
            Assert.AreEqual(vehicle2.RangeWLTP, vehicle2.RangeEPA * epaToWLTPFactor2);

            decimal averageFactor = (epaToWLTPFactor1 + epaToWLTPFactor2) / 2;
            Assert.AreEqual(averageFactor, 1.75);

            Vehicle[] vehicles = new Vehicle[] { vehicle1, vehicle2, epaOnly };

            IDictionary<Vehicle, int> wltpByVehicle = Vehicle.ComputeVehicleToWLTPRange(vehicles);

            Assert.AreEqual(wltpByVehicle[vehicle1], 150);
            Assert.AreEqual(wltpByVehicle[vehicle2], 240);
            Assert.AreEqual(wltpByVehicle[epaOnly], 200 * 1.75);
            Assert.AreEqual(wltpByVehicle[epaOnly], 200 * averageFactor);
        }

        [Test]
        public void TestGetEstimatedWLTPRangeFromNEDC()
        {
            Vehicle vehicle1 = MakeVehicle("Make", "Model", null, 100, 125);
            Vehicle vehicle2 = MakeVehicle("AnotherMake", "AnotherModel", null, 120, 240);

            Vehicle nedcOnly = MakeVehicle("EPAMake", "EPAModel", null, null, 300);

            // NEDC to WLTP factor is (100/125 + 120/240) / 2
            decimal nedcToWLTPFactor1 = vehicle1.RangeWLTP.Value / (decimal)vehicle1.RangeNEDC.Value;
            Assert.AreEqual(nedcToWLTPFactor1, 0.8m);
            Assert.AreEqual(vehicle1.RangeWLTP, vehicle1.RangeNEDC * nedcToWLTPFactor1);

            decimal nedcToWLTPFactor2 = vehicle2.RangeWLTP.Value / (decimal)vehicle2.RangeNEDC.Value;
            Assert.AreEqual(nedcToWLTPFactor2, 0.5m);
            Assert.AreEqual(vehicle2.RangeWLTP, vehicle2.RangeNEDC * nedcToWLTPFactor2);

            decimal averageFactor = (nedcToWLTPFactor1 + nedcToWLTPFactor2) / 2;
            Assert.AreEqual(averageFactor, 0.65);

            Vehicle[] vehicles = new Vehicle[] { vehicle1, vehicle2, nedcOnly };

            IDictionary<Vehicle, int> wltpByVehicle = Vehicle.ComputeVehicleToWLTPRange(vehicles);

            Assert.AreEqual(wltpByVehicle[vehicle1], 100);
            Assert.AreEqual(wltpByVehicle[vehicle2], 120);
            Assert.AreEqual(wltpByVehicle[nedcOnly], 300 * 0.65);
            Assert.AreEqual(wltpByVehicle[nedcOnly], 300 * averageFactor);
        }
    }
}

