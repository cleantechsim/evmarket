
using System.Runtime.CompilerServices;

namespace CleanTechSim.EVMarket.Helpers.Storage.AzureTableStorage
{
    public class RowKey : CompositeKeyAttribute
    {
        public RowKey([CallerLineNumber] int order = 0)
            : base(order)
        {

        }
    }
}
