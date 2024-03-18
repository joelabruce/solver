﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace solver
{
    public class _
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="T"></param>
        /// <param name="tupleSolutionProcessor"></param>
        /// <param name="tuplesOfDProcessor"></param>
        /// <returns></returns>
        public static BigInteger UniverseGenerator(BigInteger T, Action<BigInteger[]> tupleSolutionProcessor = null, Action<BigInteger, BigInteger> tuplesOfDProcessor = null)
        {
            long n = (long) Math.Floor(nFromT(T));
            BigInteger[] U = new BigInteger[n];
            BigInteger[] i = new BigInteger[n + 1];
            BigInteger count = 0;

            // Because of the way arrays are accessed and modified in the recursive function, always ascend the layers, do not descend the layers.
            // Descending will cause unexpected results.
            for (long d = 1; d <= n; d++)
            {
                var totalSolutionsForD = RecursiveTupleBI(d, T, U, i, 0, tupleSolutionProcessor);
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
        public static double nFromT(BigInteger T)
        {
            var f = (double) (2 * T) + .25;
            return Math.Sqrt(f) - .5;
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
        public static BigInteger RecursiveTupleBI(long tupleSize, BigInteger T, BigInteger[] U, BigInteger[] i, BigInteger lowerbound, Action<BigInteger[]> tupleProc)
        {
            var nextLayer = tupleSize - 1;
            U[nextLayer] = _.U_(tupleSize, T, lowerbound);
            BigInteger count = 0;

            if (nextLayer == 0)
            {
                i[0] = U[0];
                tupleProc?.Invoke(i);
                count = 1;
            }
            else
            {
                for (i[nextLayer] = i[tupleSize] + 1; i[nextLayer] <= U[nextLayer]; i[nextLayer]++)
                {
                    count += RecursiveTupleBI(tupleSize - 1, T, U, i, lowerbound + i[nextLayer], tupleProc);
                }
            }

            return count;
        }

        public static BigInteger ExperimentalTotalTriplets(BigInteger T)
        {
            var U2 = (T - 3) / 2;
            var U3 = U2 / 3;

            var u2Sum = (U2 * U2 + U2) / 2;
            var u3Sum = 3 * (U3 * U3 + U3) / 2;

            BigInteger offset = 0;

            var result = u2Sum - u3Sum + offset;

            return result;
        }

        public static BigInteger StrictPartitionTriplets(BigInteger n)
        {
            // Only works for particitions of n == 3

            var U2 = _.U_(2, n, 1);
            var U2over3 = U2 / 3;

            var U2Sum = _.s1ne(U2);
            var overshot = 3 * (U2over3 * U2over3 + U2over3) / 2;

            var r = n % 3;
            var error = r * U2over3;

            var res = U2Sum + error - overshot;

            // Why do I need to add this back on every 6th step up from T?
            // My guess is some rounding error somewhere else is creeping in
            // Need to further investigate, but for now it works.
            var zz = (n - 3) / 6;
            var rem = (n - 3) % 6;
            if (rem == 0)
            {
                res += (3 * zz);
            }

            return res;
        }

        /// <summary>
        /// BigInteger version of U.
        /// </summary>
        /// <param name="setSize"></param>
        /// <param name="T"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static BigInteger U_(BigInteger setSize, BigInteger T, BigInteger offset)
        {
            return (T - s1ne(setSize) - offset) / setSize;
        }

        /// <summary>
        /// BigInteger version of sum of i from 1 to n exclusive.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static BigInteger s1ne(BigInteger n)
        {
            return (n < 2) ? 0 : (n * n - n) / 2;
        }
    }
}
