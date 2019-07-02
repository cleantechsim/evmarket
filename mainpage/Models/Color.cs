

namespace CleanTechSim.MainPage.Models
{

    public class Color
    {

        private readonly int r, g, b;

        public int R { get { return r; } }
        public int G { get { return g; } }
        public int B { get { return b; } }


        public static readonly Color Red = new Color(0xFF, 0x00, 0x00);
        public static readonly Color Green = new Color(0x00, 0xFF, 0x00);
        public static readonly Color Blue = new Color(0x00, 0x00, 0xFF);

        public Color() { }

        public Color(int r, int g, int b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }

    }

}