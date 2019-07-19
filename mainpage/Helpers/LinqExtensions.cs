
using System;

using System.Collections.Generic;
using System.Linq;

namespace CleanTechSim.MainPage.Helpers
{
    public static class LinqExtensions
    {
        public static int Median(this IEnumerable<int> enumerable)
        {
            int median;

            int count = enumerable.Count();

            switch (count)
            {
                case 0:
                    throw new ArgumentException("Empty list");

                case 1:

                    median = enumerable.ElementAt(0);
                    break;

                default:
                    List<int> list = new List<int>(enumerable);

                    list.Sort();

                    int mid = count / 2;

                    if (count % 2 == 0)
                    {
                        median = (list.ElementAt(mid - 1) + list.ElementAt(mid)) / 2;
                    }
                    else
                    {
                        median = list.ElementAt(mid);
                    }
                    break;

            }

            return median;
        }

        public static decimal Median<T>(this IEnumerable<T> enumerable, Func<T, decimal> selector)
        {

            decimal median;

            int count = enumerable.Count();

            switch (count)
            {
                case 0:
                    throw new ArgumentException("Empty list");

                case 1:

                    median = selector.Invoke(enumerable.ElementAt(0));
                    break;

                default:
                    List<decimal> list = new List<decimal>(enumerable.Select(selector));

                    list.Sort();

                    int mid = count / 2;

                    if (count % 2 == 0)
                    {
                        median = (list.ElementAt(mid - 1) + list.ElementAt(mid)) / 2;
                    }
                    else
                    {
                        median = list.ElementAt(mid);
                    }
                    break;

            }

            return median;
        }
    }

}

