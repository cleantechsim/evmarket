
using System.Runtime.CompilerServices;

namespace CleanTechSim.MainPage.Helpers.Storage.AzureTableStorage
{
    public class RowKey : CompositeKeyAttribute
    {
        public RowKey([CallerLineNumber] int order = 0)
            : base(order)
        {

        }
    }
}
