

using System.Collections.Generic;

using CleanTechSim.MainPage.Models;

namespace CleanTechSim.MainPage.Helpers.Model
{
    public interface IGraphModelType<T>
    {
        string Title { get; }

        DataPointFormat DataPointFormat { get; }

        decimal GetDataPointX(T instance);

        decimal GetDataPointY(T instance);

        IEnumerable<DataSource> GetSources(T instance);
    }

    public interface ISingleLineGraphModelType<T>
        : IGraphModelType<T>
    {

    }

    public interface IMultiLineGraphModelType<T, KEY>
        : IGraphModelType<T>
    {
        IDictionary<KEY, List<T>> GetByDistinctKeys(IEnumerable<T> instances);

        string GetLineLabel(KEY key);
    }
}
