
using System;
using System.Collections.Generic;

using CleanTechSim.EVMarket.Models.Helper.GraphData;

namespace CleanTechSim.EVMarket.Models.Helper.ClientGraph
{
    public class DataSet
    {
        public string Label { get; }

        public Color Color { get; }

        public IEnumerable<decimal?> Values { get; }


        public DataSet(string label, Color color, List<decimal?> values)
        {
            if (color == null)
            {
                throw new ArgumentNullException();
            }

            if (values == null)
            {
                throw new ArgumentNullException();
            }

            this.Label = label;
            this.Color = color;
            this.Values = values;
        }

        public bool HasLabel()
        {
            return !string.IsNullOrWhiteSpace(Label);
        }
    }

}
