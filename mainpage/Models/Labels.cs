
using System.Collections.Generic;

namespace CleanTechSim.MainPage.Models
{
    public class Labels
    {
        public static List<string> From(params string[] strings)
        {
            return new List<string>(strings);
        }

        public List<string> List { get; set; }

        public Labels()
        {

        }

        public Labels(params string[] labels)
        {
            this.List = new List<string>(labels);
        }
    }
}