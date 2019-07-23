
using System.Runtime.CompilerServices;

namespace CleanTechSim.EVMarket.Helpers.Storage.AzureTableStorage
{
    public class PartitionKey : CompositeKeyAttribute
    {
        public PartitionKey([CallerLineNumber] int order = 0)
            : base(order)
        {

        }
    }
}
