
using System.Collections.Generic;
using System.Linq;

using CleanTechSim.MainPage.Helpers.Storage.AzureTableStorage;

namespace CleanTechSim.MainPage.Models.Persistent
{
    public class Vehicle : BasePersistentModel
    {
        [PartitionKey]
        public string Make { get; set; }

        [RowKey]
        public string Model { get; set; }

        public int? Year { get; set; }

        public decimal? PriceDollars { get; set; }
        public decimal? PriceEuros { get; set; }

        public decimal? BatteryCapacity { get; set; }
        public int? RangeEPA { get; set; }
        public int? RangeWLTP { get; set; }
        public int? RangeNEDC { get; set; }

        public int? TopSpeed { get; set; }
        public decimal? Acceleration { get; set; }

        public decimal? Power { get; set; }
        public decimal Torque { get; set; }

        public decimal OnboardCharger { get; set; }
        public int? Fast20To80 { get; set; }

        public delegate int? GetRange(Vehicle vehicle);


        public static Dictionary<Vehicle, int> ComputeVehicleToWLTPRange(IEnumerable<Vehicle> vehicles)
        {
            IEnumerable<Vehicle> vehiclesWithAllRangesSet = vehicles.Where(vehicle =>
                       vehicle.RangeEPA.HasValue
                    && vehicle.RangeWLTP.HasValue
                    && vehicle.RangeNEDC.HasValue);


            IEnumerable<Vehicle> carsWithEPAAndWLTP = from Vehicle vehicle in vehicles
                                                      where vehicle.RangeEPA.HasValue && vehicle.RangeWLTP.HasValue
                                                      select vehicle;


            double? epaToWLTPConversionRate = carsWithEPAAndWLTP.Any()
                ? carsWithEPAAndWLTP.Average(vehicle => vehicle.RangeWLTP.Value / (double)vehicle.RangeEPA.Value)
                : (double?)null;

            IEnumerable<Vehicle> carsWithNEDCAndWLTP = from Vehicle vehicle in vehicles
                                                       where vehicle.RangeNEDC.HasValue && vehicle.RangeWLTP.HasValue
                                                       select vehicle;

            double? nedcToWLTPConversionRate = carsWithNEDCAndWLTP.Any()
                ? carsWithNEDCAndWLTP.Average(vehicle => vehicle.RangeWLTP.Value / (double)vehicle.RangeNEDC.Value)
                : (double?)null;

            Dictionary<Vehicle, int> rangeWLTP = new Dictionary<Vehicle, int>();

            IEnumerable<Vehicle> carsWithWLTP = from Vehicle vehicle in vehicles where vehicle.RangeWLTP.HasValue select vehicle;
            IEnumerable<Vehicle> carsWithoutWLTP = from Vehicle vehicle in vehicles where !vehicle.RangeWLTP.HasValue select vehicle;

            foreach (Vehicle vehicle in carsWithWLTP)
            {
                rangeWLTP[vehicle] = vehicle.RangeWLTP.Value;
            }

            foreach (Vehicle vehicle in carsWithoutWLTP)
            {
                if (vehicle.RangeEPA.HasValue)
                {
                    rangeWLTP[vehicle] = (int)(vehicle.RangeEPA.Value * epaToWLTPConversionRate);
                }
                else if (vehicle.RangeNEDC.HasValue)
                {
                    rangeWLTP[vehicle] = (int)(vehicle.RangeNEDC.Value * nedcToWLTPConversionRate);
                }
            }

            return rangeWLTP;
        }

        public override int GetHashCode()
        {
            return Make.GetHashCode() + Model.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            bool equals;

            if (!(obj is Vehicle))
            {
                equals = false;
            }
            else
            {
                Vehicle other = (Vehicle)obj;

                equals = this.Make.Equals(other.Make) && this.Model.Equals(other.Model);
            }

            return equals;
        }
    }
}
