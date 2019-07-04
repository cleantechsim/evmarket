
using System.Collections.Generic;
using System.Linq;

using CleanTechSim.MainPage.Models;
using CleanTechSim.MainPage.Models.Persistent;

namespace CleanTechSim.MainPage.Helpers.Model
{
    public delegate KEY GetKey<T, KEY>(T instance);
    public delegate decimal GetX<T>(T instance);
    public delegate decimal GetY<T>(T instance);
    public delegate string GetLabel<KEY>(KEY key);

    public abstract class GraphModelType<T>
        : IGraphModelType<T> where T : BasePersistentModel
    {

        private readonly GetX<T> getX;
        private readonly GetY<T> getY;

        public string Title { get; }
        public DataPointFormat DataPointFormat { get; }

        public GraphModelType(
            string title,
            Encoding xEncoding,
            Encoding yEncoding,
            GetX<T> getX,
            GetY<T> getY)
        {
            this.Title = title;
            this.DataPointFormat = new DataPointFormat(xEncoding, yEncoding);

            this.getX = getX;
            this.getY = getY;
        }


        public decimal GetDataPointX(T instance)
        {
            return getX.Invoke(instance);
        }

        public decimal GetDataPointY(T instance)
        {
            return getY.Invoke(instance);
        }

        public IEnumerable<DataSource> GetSources(T instance)
        {
            return instance.Sources.Select(source => new DataSource(source));
        }
    }

    public class SingleLineGraphModelType<T>
        : GraphModelType<T>, ISingleLineGraphModelType<T> where T : BasePersistentModel
    {
        public SingleLineGraphModelType(
            string title,
            Encoding xEncoding,
            Encoding yEncoding,
            GetX<T> getX,
            GetY<T> getY)

            : base(title, xEncoding, yEncoding, getX, getY)
        {

        }
    }

    public class MultiLineGraphModelType<T, KEY>
        : GraphModelType<T>, IMultiLineGraphModelType<T, KEY> where T : BasePersistentModel
    {
        private readonly GetKey<T, KEY> getKey;
        private readonly GetLabel<KEY> getLabel;

        public MultiLineGraphModelType(
            string title,
            Encoding xEncoding,
            Encoding yEncoding,
            GetX<T> getX,
            GetY<T> getY,
            GetKey<T, KEY> getKey,
            GetLabel<KEY> getLabel)
            : base(title, xEncoding, yEncoding, getX, getY)
        {
            this.getKey = getKey;
            this.getLabel = getLabel;
        }

        public IDictionary<KEY, List<T>> GetByDistinctKeys(IEnumerable<T> instances)
        {
            Dictionary<KEY, List<T>> dictionary = new Dictionary<KEY, List<T>>();

            foreach (T instance in instances)
            {
                KEY key = getKey.Invoke(instance);

                List<T> instancesForKey;

                if (!dictionary.TryGetValue(key, out instancesForKey))
                {
                    instancesForKey = new List<T>();

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