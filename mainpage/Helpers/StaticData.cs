
using CleanTechSim.MainPage.Models;

namespace CleanTechSim.MainPage.Helpers
{


    public class StaticData
    {

        public static LineGraph EvAdoption = new LineGraph(new Line(
            "Norway",
            Color.Green,

            new DataSeries(
                new DataPoint(2017.12m, 27.5m, "https://cleantechnica.com/2018/01/04/50-new-car-registrations-norway-2017-plug-vehicles-hybrids"),
                new DataPoint(2018.09m, 45m, "https://electrek.co/2018/10/01/electric-vehicle-sales-new-record-norway-tesla"),
                new DataPoint(2019.01m, 37.8m, "https://gronnkontakt.no/2019/02/elbilsalget-i-januar"),
                new DataPoint(2019.02m, 40.7m, "https://www.elbil24.no/nyheter/her-er-vinnerne-og-taperne/70831731"),
                new DataPoint(2019.03m, 58.4m,
                    "https://elbil.no/norway-reaches-historic-electric-car-market-share",
                    "https://www.electrive.com/2019/04/02/58-4-of-all-new-vehicles-in-norway-are-purely-electric")
            )
        ),

        new Line(
            "China",
            Color.Red,

            new DataSeries(
                new DataPoint(2017.12m, 3.3m, "https://cleantechnica.com/2018/01/29/2017-china-electric-car-sales-blow-world-water-baic-ec-series-superstar"),
                new DataPoint(2018.01m, 1.4m, "https://cleantechnica.com/2018/03/03/china-electric-car-sales-january-baic-ec-series-cadillac-shine"),
                new DataPoint(2019.01m, 4.8m, "https://insideevs.com/news/343009/in-january-plug-in-ev-car-sales-in-china-almost-tripled"),
                new DataPoint(2019.02m, 4.3m, "https://insideevs.com/news/343600/plug-in-electric-car-sales-increased-in-china-in-february"),
                new DataPoint(2019.03m, 5.8m, "https://insideevs.com/news/346882/march-2019-sales-china")
            )
        )

        );

        static StaticData()
        {

        }
    }
}
