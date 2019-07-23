

using System.Collections.Generic;

using CleanTechSim.EVMarket.Models;

namespace CleanTechSim.EVMarket.Models.Helper.GraphData.Prepare
{
    public interface IGraphModelType<INPUT, PREPARED>
    {
        string Title { get; }

        string SubTitle { get; }

        DataPointFormat DataPointFormat { get; }

        int NumLines { get; }

        PREPARED Prepare(INPUT input);

        int GetNumX(INPUT input, PREPARED prepared);

        string GetLineLabel(int line);

        decimal GetDataPointX(INPUT input, PREPARED prepared, int line, int index);

        decimal? GetDataPointY(INPUT input, PREPARED prepared, int line, int index);

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
