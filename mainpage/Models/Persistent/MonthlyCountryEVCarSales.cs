
using CleanTechSim.MainPage.Helpers.Storage.AzureTableStorage;

namespace CleanTechSim.MainPage.Models.Persistent
{

    public class MonthlyCountryEVCarSales : BasePersistentModel
    {

        [PartitionKey]
        public string Country { get; set; }

        [RowKey]
        public int Year { get; set; }

        [RowKey]
        public int Month { get; set; }

        public decimal SalesPercent { get; set; }
    }

}
