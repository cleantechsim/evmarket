
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

    public class GraphModelType<T, KEY> : IGraphModelType<T, KEY> where T : BasePersistentModel
    {

        private readonly GetKey<T, KEY> getKey;
        private readonly GetX<T> getX;
        private readonly GetY<T> getY;
        private readonly GetLabel<KEY> getLabel;

        public string Title { get; }
        public DataPointFormat DataPointFormat { get; }

        public GraphModelType(
            string title,
            Encoding xEncoding,
            Encoding yEncoding,
            GetKey<T, KEY> getKey,
            GetX<T> getX,
            GetY<T> getY,
            GetLabel<KEY> getLabel
            )
        {
            this.Title = title;
            this.DataPointFormat = new DataPointFormat(xEncoding, yEncoding);

            this.getKey = getKey;
            this.getX = getX;
            this.getY = getY;
            this.getLabel = getLabel;
        }

        public IDictionary<KEY, List<T>> GetByDistinctKeys(IEnumerable<T> entities)
        {
            Dictionary<KEY, List<T>> dictionary = new Dictionary<KEY, List<T>>();

            foreach (T entity in entities)
            {

                KEY key = getKey.Invoke(entity);

                List<T> keyEntities;

                if (!dictionary.TryGetValue(key, out keyEntities))
                {
                    keyEntities = new List<T>();

                    dictionary[key] = keyEntities;
                }

                keyEntities.Add(entity);
            }

            return dictionary;
        }

        public decimal GetDataPointX(T instance)
        {
            return getX.Invoke(instance);
        }

        public decimal GetDataPointY(T instance)
        {
            return getY.Invoke(instance);
        }

        public string GetLineLabel(KEY key)
        {
            return getLabel.Invoke(key);
        }

        public IEnumerable<DataSource> GetSources(T instance)
        {
            return instance.Sources.Select(source => new DataSource(source));
        }
    }
}