
using System;
using System.Collections.Generic;
using System.Linq;

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("mainpage_test")]
namespace CleanTechSim.MainPage.Helpers
{
    internal delegate int Compare(decimal index);

    internal class BinarySearch
    {

        internal static decimal Search(decimal start, decimal num, Compare comparer)
        {
            decimal foundIndex;

            if (num == 0)
            {
                foundIndex = -1;
            }
            else
            {
                decimal sectionOffset = num / 2;
                decimal mid = start + sectionOffset;

                /*
                bool hasNextValue = num - sectionOffset > 1;
                T nextValue = hasNextValue ? array[mid + 1] : -1L;
                */

                int val = comparer.Invoke(mid);

                Console.WriteLine("## test with mid " + mid);

                switch (val)
                {
                    case -1:
                        foundIndex = Search(start, sectionOffset, comparer);
                        break;

                    case 0:
                        foundIndex = mid;
                        break;

                    case 1:
                        foundIndex = Search(mid, num - sectionOffset - 1, comparer);
                        break;

                    default:
                        throw new InvalidOperationException();
                }
            }

            return foundIndex;
        }
    }

}