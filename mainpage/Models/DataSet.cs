
using System;
using System.Collections.Generic;

namespace CleanTechSim.MainPage.Models
{
    public class DataSet
    {
        private readonly string label;
        private readonly Color color;
        private readonly List<decimal?> values;

        public string Label { get { return label; } }

        public Color Color { get { return color; } }

        public IEnumerable<decimal?> Values { get { return values; } }


        public DataSet(string label, Color color, List<decimal?> values)
        {
            if (label == null)
            {
                throw new ArgumentNullException();
            }

            if (color == null)
            {
                throw new ArgumentNullException();
            }

            if (values == null)
            {
                throw new ArgumentNullException();
            }

            this.label = label;
            this.color = color;
            this.values = values;
        }
    }

}
