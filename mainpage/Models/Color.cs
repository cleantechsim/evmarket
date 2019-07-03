
using System.Collections.Generic;

namespace CleanTechSim.MainPage.Models
{
    public class Color
    {
        public int R { get; }
        public int G { get; }
        public int B { get; }

        public static readonly Color Red = new Color(0xFF, 0x00, 0x00);
        public static readonly Color Green = new Color(0x00, 0xFF, 0x00);
        public static readonly Color Blue = new Color(0x30, 0x85, 0xFF);

        public static readonly IEnumerable<Color> Colors = new Color[] {
            Green,
            Blue,
            Red
        };

        public Color() { }

        public Color(int r, int g, int b)
        {
            this.R = r;
            this.G = g;
            this.B = b;
        }
    }
}