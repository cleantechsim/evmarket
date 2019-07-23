
using CleanTechSim.EVMarket.Helpers.Storage.AzureTableStorage;

namespace CleanTechSim.EVMarket.Models.Domain
{
    public class BatteryCost : BasePersistentModel
    {
        // Somewhat strange splitting in order to have distinct partitionkey/rowkey in table storage
        [PartitionKey]
        public int Century { get; set; }

        [RowKey]
        public int Year { get; set; }

        public decimal Cost { get; set; }
    }
}
