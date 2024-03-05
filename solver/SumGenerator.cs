using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solver
{
    public class SumGenerator
    {
        /// <summary>
        /// Using the Pigeonhole Principle, calculates the maximum set size of items that can be used to sum to T.
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        public static double nFromT(int T)
        {
            return Math.Sqrt(2f * T + .25f) - .5f;
        }

        /// <summary>
        /// Calculates the upper bound for possible values of an item in a set given T, set size, and the sum of the previous values in the set.
        /// </summary>
        /// <param name="T"></param>
        /// <param name="setSize"></param>
        /// <param name="cumulativePrior"></param>
        /// <returns></returns>
        public static int UpperBounds(int T, int setSize, int cumulativePrior = 0)
        {
            return (int) MathF.Floor((T - cumulativePrior - nSum(setSize - 1)) / setSize);
        }

        /// <summary>
        /// Calculates the sum of i from i = 1 to n.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int nSum(int n)
        {
            return (n * n + n) / 2;
        }
    }
}
