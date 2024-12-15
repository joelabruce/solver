using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace solver
{
    /// <summary>
    /// Foundational Math Functions.
    /// Shortened to _ to highlight these functions are so widespread and fundamental.
    /// </summary>
    public static class _
    {
        /// <summary>
        /// Generates all subsets that sum to T.
        /// </summary>
        /// <param name="T"></param>
        /// <param name="tupleSolutionProcessor"></param>
        /// <param name="tuplesOfDProcessor"></param>
        /// <returns></returns>
        public static BigInteger UniverseGenerator(BigInteger T, Action<BigInteger[]> tupleSolutionProcessor = null, Action<BigInteger, BigInteger> tuplesOfDProcessor = null)
        {
            long n = (long)Math.Floor(nFromT(T));
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
            var actualThreads = (desiredThreadCount < upperBound) ? desiredThreadCount : (upperBound != 0) ? upperBound : 1;
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
            var f = (double)(2 * T) + .25;
            return Math.Sqrt(f) - .5;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <returns>The T to generate tuples from, which can be used to solve for partitions of given n.</returns>
        public static BigInteger TforPartitionsOfN(BigInteger n)
        {
            return (n * n + 3 * n + 2) / 2 - 1;
        }

        /// <summary>
        /// Generates all tuples that sum to T for a given tupleSize.
        /// </summary>
        /// <param name="tupleSize">How many elements to have in resulting tuples.</param>
        /// <param name="T">Target sum.</param>
        /// <param name="U">Upperbound for a given dimension.</param>
        /// <param name="i">Index for a given dimension.</param>
        /// <param name="lowerbound">Accumulated lower bounds.</param>
        /// <param name="tuplesolutionProcessor">Action to take on solution tuple.</param>
        /// <returns></returns>
        public static BigInteger RecursiveTupleBI(
            long tupleSize,
            BigInteger T,
            BigInteger[] U,
            BigInteger[] i,
            BigInteger lowerbound,
            Action<BigInteger[]> tupleProc
        )
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

        public static BigInteger TestQuadrupletsSolver(BigInteger T)
        {
            var n = (T - 6) / 4;
            var inner = (T + 2) / 4;


            //BigInteger result = n * SimpleTriplets(-4 * inner + 44 * n;

            return 0;
        }

        // Predictions based on preliminary results
        //n mod 4 = 1:   0,   0,   0,   3,   8,  16,  27,  40,  56,  75,  96, 120, 147, 176, 208, 243, 280, 320, ...
        //n mod 4 = 2:   0,   0,   1,   4,  10,  19,  30,  44,  61,  80, 102, 127, 154, 184, 217, 252, 290, ...
        //n mod 4 = 3:   0,   0,   1,   5,  12,  21,  33,  48,  65,  85, 108, 133, 161, 192, 225, 261, 300, ...
        //n mod 4 = 0:   0,   0,   2,   7,  14,  24,  37,  52,  70,  91, 114, 140, 169, 200, 234, 271, 310, ...
        //if (r == 0) result = k * k + 2 * k - 1;           //
        //if (r == 1) result = 3 * k * k / 2 + k / 2 + 1;   //
        //if (r == 2) result = 3 * (k * k - k) / 2 ;        //
        //if (r == 3) result = 3 * (k * k) - k / 2;   //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        public static BigInteger PhasedHexagonalQuadruplets(BigInteger T)
        {
            var n = (T - 6) / 4;
            var offset = (T + 2) % 4;

            BigInteger result = 0;
            //for (BigInteger n = T - 4; n >= 6; n -= 4)    // This was the pattern discovered.
            for (BigInteger i = 1; i <= n; i++)              // Another way of writing the nth term of phased pentagonal
            {
                result += SimpleTriplets((i - 1) * 4 + 6 + offset);
            }

            return result;
        }

        /// <summary>
        /// Calculates the strict partitions for triplets of T in linear time.
        /// Purposely left unoptimized to show math.
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        public static BigInteger SimpleTriplets(BigInteger T)
        {
            // Need to use Math.Floor to correctly floor when values become negative.
            var x = new BigInteger(Math.Floor(((double)(T - 4)) / 2));
            var UB = TriangularNumber(x);     // Upper bounds of T triplets, from which we subtract.

            BigInteger result = UB - JoelSeries(T);
            return result;
        }

        /// <summary>
        /// A series of numbers to be used to subtract from certain Triangular numbers to get exact distinct triplets.
        /// Generates a molien series.
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        public static BigInteger JoelSeries(BigInteger T)
        {
            if (T == 1) return 1;

            // C#'s built in % operator is a remainder operator, not a true modulo operator.
            // So to make the math consistent we implemented an extension method for BigInteger.
            var r = (int)(2 - T).Mod(3);
            BigInteger n = ((T - 2 * r)) / 6;
            // Order of operations is important because of implicit floor operations for BigInteger divisions.
            BigInteger offset = r switch
            {
                0 => (3 * n * n - n) / 2,
                1 => (3 * n * n + n) / 2,
                _ => 3 * ((n * n + n) / 2)
            };

            var n2 = n + r;
            var offset2 = (3 * n2 * n2 - n2) / 2;

            Console.WriteLine(offset2 == offset);

            return offset;
        }

        /// <summary>
        /// Exactly calculates distinct triplets based on recognizing pattern in how many pairs exist within triplets.
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        public static BigInteger PhasedPentagonalTriplets(BigInteger T, bool capLowerLimit = true)
        {
            if (T < 6 && capLowerLimit) return 0;

            // Further optimizations can be made, but not optimized to show weaving pattern
            // Further research is to see if a similar pattern holds for quadruplets and other tuplets
            var r = T % 3;

            var n1 = (T / 6);
            var n2 = (T - 2) / 6;
            var n3 = (T - 4) / 6;
            var s1 = (3 * n1 * n1 - n1) / 2;        // Same as sum of i from 1 to n1 (3i - 2): 1, 4, 7, 10, 13, 16, 19, 22...
            var s2 = (3 * n2 * n2 + n2) / 2;        // Same as sum of i from 1 to n2 (3i - 1): 2, 5, 8, 11, 14, 17, 20, 23...
            var s3 = (3 * n3 * n3 + 3 * n3) / 2;    // Same as sum of i from 1 to n3 (3i - 0): 3, 6, 9, 12, 15, 18, 21, 24...

            BigInteger result = 0;

            // Determining which weaves to add can easily be seen by the pattern of pairs each triplet generates
            // 1, 2, 4, 5, 7, 8, 10, 11, ...
            if (r == 0) result = s1 + s2;     // Sum of weave 1 and 2

            // 1, 3, 4, 6, 7, 9, 10, 12, ...
            if (r == 1) result = s1 + s3;     // Sum of Weave 1 and 3

            // 2, 3, 5, 6, 8, 9, 11, 12, ...
            if (r == 2) result = s2 + s3;     // Sum of Weave 2 and 3

            // Another way of thinking about this, is that we take the triangular number, and subtract the gaps that are formed in the pattern.
            // The gaps are what make this a fractal.
            // This effectively makes the series of triangular numbers the upper bounds.
            return result;
        }

        /// <summary>
        /// While provides exact answers, less elegant to educe pattern that this logic is based on.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>        
        public static BigInteger StrictPartitionTriplets(BigInteger n)
        {
            // Only works for particitions of n == 3

            var U2 = _.U_(2, n, 1);
            var U2over3 = U2 / 3;

            var U2Sum = _.TriangularNumber(U2 - 1);
            var overshot = 3 * (U2over3 * U2over3 + U2over3) / 2;

            var r = n % 3;
            var error = r * U2over3;

            var res = U2Sum + error - overshot;

            // This is an optimization because it allows for only figuring out 2 periods and adding the 3rd period as an adjustment.
            var zz = (n - 3) / 6;
            var rem = (n - 3) % 6;
            if (rem == 0)
            {
                res += (3 * zz);
            }

            return res;
        }

        public static BigInteger StrictPartitionTripletsSub(BigInteger T)
        {
            var a = (T - 4) / 2; // new BigInteger(Math.Floor(((double)(T - 4)) / 2));
            var m = (2 - T).Mod(3);
            var j = 2 * m;
            var c = j - 1;
            var b = (T - j) / 6; // new BigInteger(Math.Floor(((double)(T - j)) / 6));

            var t = (a * a + a) / 2;
            var o = 3 * (b * b) / 2 + (c * b) / 2;

            var s = t - o;
            Console.WriteLine($"T{t} m{m} j{j} c{c} b{b} o{o}");
            return s;
        }

        /// <summary>
        /// Provides correct answer by summing up the two streams / rings.
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        public static BigInteger StrictPartitionTripletsAdd(BigInteger T)
        {
            BigInteger m1 = (1 - T).Mod(3);
            BigInteger m2 = (0 - T).Mod(3);
            BigInteger j1 = 2 * m1;
            BigInteger j2 = 2 * m2;

            // Floor operation vital to ensure rounding correctly when negative values are involved.
            BigInteger b1 = new BigInteger(Math.Floor((double)(T - j1) / 6));
            BigInteger b2 = new BigInteger(Math.Floor((double)(T - j2) / 6));

            BigInteger c1 = j1 - 1;
            BigInteger c2 = j2 - 1;

            // Order of operations vital. Divide by two at end to avoid truncation of the integer types.
            BigInteger o1 = (3 * (b1 * b1) + (c1 * b1)) / 2;
            BigInteger o2 = (3 * (b2 * b2) + (c2 * b2)) / 2;

            Debug.WriteLine(T);
            Debug.WriteLine($"m1:{m1} j1:{j1} c1:{c1} b1:{b1} o1:{o1}");
            Debug.WriteLine($"m2:{m2} j2:{j2} c2:{c2} b2:{b2} o2:{o2}");

            return o1 + o2;
        }

        /// <summary>
        /// Calculates the upper bound of a set given the setSize and offset.
        /// Since we are dealing with discreet math and BigIntegers, no need for the floor function,
        /// (The values are automatically truncated.)
        /// </summary>
        /// <param name="setSize"></param>
        /// <param name="T"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static BigInteger U_(BigInteger setSize, BigInteger T, BigInteger offset)
        {
            return (T - TriangularNumber(setSize - 1) - offset) / setSize;
        }

        /// <summary>
        /// Caclulates the nth Triangular Number.
        /// Always guaranteed to be a whole number for n >= 0
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static BigInteger TriangularNumber(BigInteger n)
        {
            return (n * n + n) / 2;
        }

        /// <summary>
        /// Calculates the nth Pentagonal Number.
        /// Always guaranteed to be a whole number for n >= 0
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static BigInteger PentagonalNumber(BigInteger n)
        {
            return (3 * n * n - n) / 2;
        }

        /// <summary>
        /// Calculates the nth Hexagonal Number.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static BigInteger HexagonalNumber(BigInteger n)
        {
            return 2 * n * n - n;
        }

        /// <summary>
        /// Calculates the nth Polygonal Number of a polygon with s sides.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static BigInteger PolygonalNumber(BigInteger s, BigInteger n)
        {
            return (s - 2) * (n * n - n) / 2 + n;
        }

        /// <summary>
        /// Mathematically correct modulo that accounts for negative numbers correctly.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static BigInteger Mod(this BigInteger self, BigInteger other)
        {
            var r = self % other;
            return r < 0 ? r + other : r;
        }
    }
}
