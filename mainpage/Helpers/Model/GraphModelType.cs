
using System.Collections.Generic;
using System.Linq;

using CleanTechSim.MainPage.Models;
using CleanTechSim.MainPage.Models.Persistent;

namespace CleanTechSim.MainPage.Helpers.Model
{
    public delegate KEY GetKey<T, KEY>(T instance);

    public delegate PREPARED Prepare<INPUT, PREPARED>(INPUT input);

    public delegate int GetNumXFromInputAndPrepared<INPUT, PREPARED>(INPUT input, PREPARED prepared);

    public delegate decimal GetXFromInputAndPrepared<INPUT, PREPARED>(INPUT input, PREPARED prepared, int index);
    public delegate decimal GetYFromInputAndPrepared<INPUT, PREPARED>(INPUT input, PREPARED prepared, int index);

    public delegate string GetLabel<KEY>(KEY key);

    public class GraphModelType<INPUT, PREPARED>
        : IGraphModelType<INPUT, PREPARED>
    {

        private readonly Prepare<INPUT, PREPARED> prepare;

        private readonly GetNumXFromInputAndPrepared<INPUT, PREPARED> getNumX;
        private readonly GetXFromInputAndPrepared<INPUT, PREPARED> getX;
        private readonly GetYFromInputAndPrepared<INPUT, PREPARED> getY;

        public string Title { get; }
        public string SubTitle { get; }
        public DataPointFormat DataPointFormat { get; }


        public GraphModelType(
            string title,
            string subTitle,

            Encoding xEncoding,
            Encoding yEncoding,

            Prepare<INPUT, PREPARED> prepare,

            GetNumXFromInputAndPrepared<INPUT, PREPARED> getNumX,
            GetXFromInputAndPrepared<INPUT, PREPARED> getX,
            GetYFromInputAndPrepared<INPUT, PREPARED> getY
            )
        {
            this.Title = title;
            this.SubTitle = subTitle;

            this.DataPointFormat = new DataPointFormat(xEncoding, yEncoding);

            this.prepare = prepare;

            this.getNumX = getNumX;
            this.getX = getX;
            this.getY = getY;
        }

        public PREPARED Prepare(INPUT input)
        {
            return prepare != null ? prepare.Invoke(input) : default(PREPARED);
        }

        public int GetNumX(INPUT input, PREPARED prepared)
        {
            return getNumX.Invoke(input, prepared);
        }

        public decimal GetDataPointX(INPUT input, PREPARED prepared, int index)
        {
            return getX.Invoke(input, prepared, index);
        }

        public decimal GetDataPointY(INPUT input, PREPARED prepared, int index)
        {
            return getY.Invoke(input, prepared, index);
        }

        public virtual IEnumerable<DataSource> GetSources(INPUT input, PREPARED prepared, int index)
        {
            return Enumerable.Empty<DataSource>();
        }
    }

    public delegate decimal GetXFromInstance<T>(T instance);
    public delegate decimal GetYFromInstance<T>(T instance);

    public class InstanceGraphModelType<INSTANCE, PREPARED>
        : GraphModelType<IEnumerable<INSTANCE>, PREPARED> where INSTANCE : BasePersistentModel
    {

        public InstanceGraphModelType(
            string title,
            string subTitle,

            Encoding xEncoding,
            Encoding yEncoding,

            GetXFromInstance<INSTANCE> getX,
            GetYFromInstance<INSTANCE> getY

        )
        : base(
            title,
            subTitle,

            xEncoding,
            yEncoding,
            null,
            (input, prepared) => input.Count(),
            (input, prepared, index) => getX.Invoke(input.ElementAt(index)),
            (input, prepared, index) => getY.Invoke(input.ElementAt(index)))
        {

        }

        internal static IEnumerable<DataSource> GetSourcesFromInstances<T>(IEnumerable<T> instances, int index)
            where T : BasePersistentModel
        {
            return instances.ElementAt(index).Sources.Select(source => new DataSource(source));
        }

        public override IEnumerable<DataSource> GetSources(IEnumerable<INSTANCE> input, PREPARED prepared, int index)
        {
            return GetSourcesFromInstances(input, index);
        }
    }

    public class SingleLineGraphModelType<INSTANCE>
        : SingleLineInstanceGraphModelTypeWithPrepared<INSTANCE, object>,
          ISingleLineGraphModelType<INSTANCE> where INSTANCE : BasePersistentModel
    {
        public SingleLineGraphModelType(
            string title,
            string subTitle,
            Encoding xEncoding,
            Encoding yEncoding,
            GetXFromInstance<INSTANCE> getX,
            GetYFromInstance<INSTANCE> getY)

            : base(
                title,
                subTitle,
                xEncoding,
                yEncoding,
                instances => null,
                getX,
                (input, prepared, index) => getY.Invoke(input.ElementAt(index)))
        {

        }
    }

    public class SingleLineInstanceGraphModelTypeWithPrepared<INSTANCE, PREPARED>
        : GraphModelType<IEnumerable<INSTANCE>, PREPARED>,
          ISingleLineGraphModelTypeWithPrepared<INSTANCE, PREPARED>

          where INSTANCE : BasePersistentModel
    {

        public SingleLineInstanceGraphModelTypeWithPrepared(
            string title,
            string subTitle,
            Encoding xEncoding,
            Encoding yEncoding,
            Prepare<IEnumerable<INSTANCE>, PREPARED> prepare,
            GetXFromInstance<INSTANCE> getX,
            GetYFromInputAndPrepared<IEnumerable<INSTANCE>, PREPARED> getY)

            : base(
                title,
                subTitle,
                xEncoding,
                yEncoding,
                prepare,
                (input, prepared) => input.Count(),
                (input, prepared, index) => getX.Invoke(input.ElementAt(index)),
                getY)
        {
        }

        public override IEnumerable<DataSource> GetSources(IEnumerable<INSTANCE> input, PREPARED prepared, int index)
        {
            return InstanceGraphModelType<INSTANCE, PREPARED>.GetSourcesFromInstances(input, index);
        }
    }

    public class MultiLineGraphModelType<INSTANCE, KEY>
        : InstanceGraphModelType<INSTANCE, object>,
        IMultiLineGraphModelType<INSTANCE, KEY> where INSTANCE : BasePersistentModel
    {
        private readonly GetKey<INSTANCE, KEY> getKey;
        private readonly GetLabel<KEY> getLabel;

        public MultiLineGraphModelType(
            string title,
            string subTitle,
            Encoding xEncoding,
            Encoding yEncoding,
            GetXFromInstance<INSTANCE> getX,
            GetYFromInstance<INSTANCE> getY,
            GetKey<INSTANCE, KEY> getKey,
            GetLabel<KEY> getLabel)
            : base(
                title,
                subTitle,
                xEncoding,
                yEncoding,
                getX,
                getY)
        {
            this.getKey = getKey;
            this.getLabel = getLabel;
        }

        public IDictionary<KEY, List<INSTANCE>> GetByDistinctKeys(IEnumerable<INSTANCE> instances)
        {
            Dictionary<KEY, List<INSTANCE>> dictionary = new Dictionary<KEY, List<INSTANCE>>();

            foreach (INSTANCE instance in instances)
            {
                KEY key = getKey.Invoke(instance);

                List<INSTANCE> instancesForKey;

                if (!dictionary.TryGetValue(key, out instancesForKey))
                {
                    instancesForKey = new List<INSTANCE>();

                    dictionary[key] = instancesForKey;
                }

                instancesForKey.Add(instance);
            }

            return dictionary;
        }

        public string GetLineLabel(KEY key)
        {
            return getLabel.Invoke(key);
        }
    }
}

