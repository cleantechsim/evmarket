

using System.Collections.Generic;

using CleanTechSim.MainPage.Models;

namespace CleanTechSim.MainPage.Helpers.Model
{
    public interface IGraphModelType<T, KEY>
    {
        string Title { get; }

        DataPointFormat DataPointFormat { get; }

        IDictionary<KEY, List<T>> GetByDistinctKeys(IEnumerable<T> instances);

        string GetLineLabel(KEY key);

        decimal GetDataPointX(T instance);

        decimal GetDataPointY(T instance);

        IEnumerable<DataSource> GetSources(T instance);
    }
}
