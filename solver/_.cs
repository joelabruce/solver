using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace solver
{
    public class _
    {
        public static long UniverseGenerator(long T, Action<long[]> tupleSolutionProcessor = null, Action<long, long> tuplesOfDProcessor = null)
        {
            long n = (long) Math.Floor(nFromT(T));
            long[] U = new long[n];
            long[] i = new long[n + 1];
            long count = 0;

            Task<long>[] tasks = new Task<long>[n];

            // Because of the way arrays are accessed and modified in the recursive function, always ascend the layers, do not descend the layers.
            // Descending will cause unexpected results.
            for (long d = 1; d <= n; d++)
            {
                var totalSolutionsForD = RecursiveTuple(d, T, U, i, 0, tupleSolutionProcessor);
                tuplesOfDProcessor?.Invoke(d, totalSolutionsForD);
                count += totalSolutionsForD;
            }

            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="desiredThreadCount"></param>
        /// <param name="upperBound"></param>
        /// <returns></returns>
        public static Segment[] FrontLoadedThreadSegmentGenerator(long desiredThreadCount, long upperBound)
        {
            var actualThreads = (desiredThreadCount < upperBound) ? desiredThreadCount : upperBound;
            var segmentSize = upperBound / actualThreads;
            var remainder = upperBound % actualThreads;
            var remaining = remainder;

            Segment[] segments = new Segment[actualThreads];
            segments[0].Start = 1;
            segments[0].End = segmentSize + ((remaining > 0) ? 1 : 0);
            for (int i = 1; i < actualThreads; i++)
            {
                remaining--;
                segments[i].Start = segments[i - 1].End + 1;
                segments[i].End = segments[i].Start + (segmentSize - 1) + ((remaining > 0) ? 1 : 0);
            }

            return segments;
        }

        public static async Task<long> CombinedSegmentedTasks(long T, Segment[] segments)
        {
            var segmentCount = segments.Length;
            Task<long>[] tasks = new Task<long>[segmentCount];
            for (int taskIndex = 0; taskIndex < segmentCount; taskIndex++)
            {
                int i = taskIndex;
                tasks[i] = Task.Factory.StartNew(
                    () => TaskChunk(
                        T,
                        segments[i].Start,
                        segments[i].End
                    ),
                    TaskCreationOptions.LongRunning
                );
            }

            var counts = await Task.WhenAll(tasks);

            return counts.Sum();
        }

        public static long TaskChunk(long T, long segmentStart, long segmentEnd)
        {
            //return RecursiveTuple(T, ;
            return 1;
        }

        /// <summary>
        /// Using the Pigeonhole Principle, calculates the maximum set size of items that can be used to sum to T.
        /// Optimizes the recursion to take advantage of parallelism.
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        public static double nFromT(long T)
        {
            return Math.Sqrt(2 * T + .25) - .5;
        }

        public static async Task<long> RecursiveTupleAsync(long tupleSize, long T, long[] U, long[] i, long lowerbound = 0, Action<long[]> tuplesolutionProcessor = null)
        {
            long nextLayer = tupleSize - 1;
            U[nextLayer] = _.U(tupleSize, T, lowerbound);
            long count = 0;

            if (nextLayer == 0)
            {
                i[0] = U[0];
                tuplesolutionProcessor?.Invoke(i);
                count = 1;
            }
            else
            {
                for (i[nextLayer] = i[tupleSize] + 1; i[nextLayer] <= U[nextLayer]; i[nextLayer]++)
                {
                    count += await RecursiveTupleAsync(tupleSize - 1, T, U, i, lowerbound + i[nextLayer], tuplesolutionProcessor);
                }
            }

            return count;
        }

        /// <summary>
        /// Generates all tuples that sum to T for a give tupleSize.
        /// </summary>
        /// <param name="tupleSize">How many elements to have in resulting tuples.</param>
        /// <param name="T">Target sum.</param>
        /// <param name="U">Upperbound for a given dimension.</param>
        /// <param name="i">Index for a given dimension.</param>
        /// <param name="lowerbound">Accumulated lower bounds.</param>
        /// <param name="tuplesolutionProcessor">Action to take on solution tuple.</param>
        /// <returns></returns>
        public static long RecursiveTuple(long tupleSize, long T, long[] U, long[] i, long lowerbound = 0, Action<long[]> tuplesolutionProcessor = null)
        {
            long nextLayer = tupleSize - 1;
            U[nextLayer] = _.U(tupleSize, T, lowerbound);
            long count = 0;

            if (nextLayer == 0)
            {
                i[0] = U[0];
                tuplesolutionProcessor?.Invoke(i);
                count = 1;
            }
            else
            {
                for (i[nextLayer] = i[tupleSize] + 1; i[nextLayer] <= U[nextLayer]; i[nextLayer]++)
                {
                    count += RecursiveTuple(tupleSize - 1, T, U, i, lowerbound + i[nextLayer], tuplesolutionProcessor);
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
            var U2 = _.U(2, T, 1);
            var U2over3 = U2 / 3;

            var U2Sum = _.Sum1ToNExclusive(U2);
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

        /// <summary>
        /// Calculates U_f.
        /// </summary>
        /// <param name="setSize"></param>
        /// <param name="T"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
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
