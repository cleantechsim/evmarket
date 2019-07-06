

using System.Collections.Generic;

using CleanTechSim.MainPage.Models;

namespace CleanTechSim.MainPage.Helpers.Model
{
    public interface IGraphModelType<INPUT, PREPARED>
    {
        string Title { get; }

        DataPointFormat DataPointFormat { get; }

        PREPARED Prepare(INPUT input);

        int GetNumX(INPUT input, PREPARED prepared);

        decimal GetDataPointX(INPUT input, PREPARED prepared, int index);

        decimal GetDataPointY(INPUT input, PREPARED prepared, int index);

        IEnumerable<DataSource> GetSources(INPUT input, PREPARED prepared, int index);
    }

    public interface ISingleLineGraphModelType<INSTANCE>
        : ISingleLineGraphModelTypeWithPrepared<INSTANCE, object>
    {

    }

    public interface ISingleLineGraphModelTypeWithPrepared<INSTANCE, PREPARED>
        : IGraphModelType<IEnumerable<INSTANCE>, PREPARED>
    {

    }

    public interface IMultiLineGraphModelType<INSTANCE, KEY>
        : IGraphModelType<IEnumerable<INSTANCE>, object>
    {
        IDictionary<KEY, List<INSTANCE>> GetByDistinctKeys(IEnumerable<INSTANCE> instances);

        string GetLineLabel(KEY key);
    }
}
