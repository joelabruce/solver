using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
        public static double nFromT(long T)
        {
            return Math.Sqrt(2f * T + .25f) - .5f;
        }

        /// <summary>
        /// Universe Generating Function.
        /// </summary>
        /// <param name="tupleSize">How many elements to have in resulting tuples.</param>
        /// <param name="T">Target sum.</param>
        /// <param name="U">Upperbound for a given dimension.</param>
        /// <param name="i">Index for a given dimension.</param>
        /// <param name="lowerbound">Accumulated lower bounds.</param>
        /// <param name="write">Outputs to console if true.</param>
        /// <returns></returns>
        public static long RecursiveTuple(long tupleSize, long T, long[] U, long[] i, long lowerbound = 0, bool write = false)
        {
            long nextLayer = tupleSize - 1;
            U[nextLayer] = SumGenerator.U(tupleSize, T, lowerbound);
            long count = 0;

            if (nextLayer == 0)
            {
                i[0] = U[0];
                if (write) Console.WriteLine(string.Join(", ", i));
                count = 1;
            }
            else
            {
                for (i[nextLayer] = i[tupleSize] + 1; i[nextLayer] <= U[nextLayer]; i[nextLayer]++)
                {
                    count += RecursiveTuple(tupleSize - 1, T, U, i, lowerbound + i[nextLayer], write);
                }
            }

            return count;
        }

        /// <summary>
        /// Given patterns in how triplets are generated, predicts accuratelyup to T = 124.
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        public static long TotalTriplets(long T)
        {
            var U2 = SumGenerator.U(2, T, 1);
            var U2over3 = U2 / 3;

            var U2Sum = SumGenerator.Sum1ToNExclusive(U2);
            var overshot = 3 * (U2over3 * U2over3 + U2over3) / 2;

            var r = T % 3;
            var error = r * U2over3;

            var res = U2Sum + error - overshot;

            // Why do I need to add this back on every 6th step up from T?
            // My guess is some rounding error somewhere else is creeping in
            // Need to further investigate, but for now it works.
            var zz = (T - 3) / 6;
            var rem = (T - 3) % 6;
            if (rem == 0)
            {
                res += (3 * zz);
            }

            return res;
        }

        /// <summary>
        /// Calculates the upper bound for possible values of an item in a set given T, set size, and the sum of the previous values in the set.
        /// </summary>
        /// <param name="T"></param>
        /// <param name="setSize"></param>
        /// <param name="offset">Must be less than offset.</param>
        /// <returns></returns>
        public static long UpperBounds(long T, long setSize, long offset = 0)
        {
            return (long) MathF.Floor((T - Sum1ToNExclusive(setSize) - offset) / setSize);
        }

        public static long U(long setSize, long T, long offset)
        {
            return (long) MathF.Floor((T - Sum1ToNExclusive(setSize) - offset) / setSize);
        }

        /// <summary>
        /// Calculates the sum of i from i = 1 to n.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static long Sum1ToN(long n)
        {
            return (n * n + n) / 2;
        }

        /// <summary>
        /// Calculates the sum of i from i = 1 to n - 1.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static long Sum1ToNExclusive(long n)
        {
            return (n < 2) ? 0 : (n * n - n) / 2;
        }
    }
}
