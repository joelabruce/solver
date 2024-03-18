using solver.PMath;
using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace solver
{
    class Program
    {
        static void Main(string[] args)
        {
            // Anything larger will result in numeric overflow
            //var (ms, r) = Timer(() => TripletsAsync(12, 100000000000, 0).Result); // 3229849966403158016 - Incorrect answer because of overflow
            //var (ms, r) = Timer(() => TripletsAsync(12, 10000000000, 0).Result);  // 8333333328333343872 - Might be correct, or could be off due to overflow
            //var (ms, r) = Timer(() => TripletsAsync(12, 1000000000, 0).Result);   // 83333332833333456 - Probably wrong due to overflow (83333333333333333)
            //var (ms, r) = Timer(() => TripletsAsync(12, 100000000, 0).Result);   // 833333283333334 - Probably wrong due to rounding errors

            long T = (long) Math.Pow(10, 10);
            long tolerance = 100;
            const long desiredTaskCount = 4;
            var tms = Timed(() =>
            {
                // 3 segments seems to be the magic number for performance
                for (long i = T; i <= T + tolerance; i++)
                {
                    var j = i;

                    Console.WriteLine(j);
                    var (ms, r) = TimedFunc(() => TripletsAsync(desiredTaskCount, j).Result);
                    Console.WriteLine($"{r} in {ms}ms");

                    var (msf, rf) = TimedFunc(() => _.StrictPartitionTriplets(j));
                    Console.WriteLine($"{rf} in {msf}ms (e)");
                }
            });

            Console.WriteLine($"Total ms: {tms}, Avg: {tms / tolerance}");
            //return;

            //GenerateAllSolutionsForT(1, 123);

            //sw.Stop();
            //Console.WriteLine(x);
            //Console.WriteLine($"In {sw.ElapsedMilliseconds} ms");

            // All the triplet totals
            //for (long T = 9; T <= 100000; T += 9)
            //{
            //    var res = SumGenerator.TotalTriplets(T);

            //    Console.WriteLine($"{T},,{res}");
            //}
        }

        public static (long, T) TimedFunc<T>(Func<T> process)
        {
            Stopwatch sw = new();
            sw.Start();
            T r = process.Invoke();
            sw.Stop();

            return (sw.ElapsedMilliseconds, r);
        }

        public static long Timed(Action process)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            process.Invoke();
            sw.Stop();

            return sw.ElapsedMilliseconds;
        }

        public static void GenerateAllSolutionsForT(long start, long end) {
            for (long T = start; T <= end; T++)
            {
                var n = _.nFromT(T);

                Console.Write($"{T},,{n},{Math.Pow(2, n)},");
                _.UniverseGenerator(T, null, (d, solutions) =>
                {
                    Console.Write($",{solutions}");
                });

                Console.WriteLine();
            }
        }

        public static void OperandTest()
        {
            Operand[] operands = new Operand[5];
            operands[0] = 5f._("a") + 10f._("b"); 
            operands[1] = 17f._("c");
            operands[2] = operands[0] * operands[1];

            Console.WriteLine(operands[2]);
            Console.WriteLine(operands[2].ToExpression());
        }

        public static async Task<BigInteger> TripletsAsync(long threads, BigInteger T)
        {
            var bound3 = _.U_(3, T, 0);

            if (bound3 > long.MaxValue)
            {
                Console.WriteLine($"{long.MaxValue} $ {bound3.ToString()}");
                Console.WriteLine($"{bound3 > long.MaxValue}");
            }

            Segment[] segments = _.FrontLoadedThreadSegmentGenerator(threads, (long) bound3);
            var segmentCount = segments.Length;

            Task<BigInteger>[] tasks = new Task<BigInteger>[segmentCount];
            for (int taskIndex = 0; taskIndex < segmentCount; taskIndex++)
            {
                int i = taskIndex;
                tasks[i] = Task.Factory.StartNew(
                    () => TripletChunk(
                        T,
                        segments[i].Start,
                        segments[i].End
                    ),
                    TaskCreationOptions.LongRunning
                );
            }

            // Asynchronously
            var counts = await Task.WhenAll(tasks);

            BigInteger result = 0;
            foreach(var count in counts)
            {
                result += count;
            }

            return result;
        }

        public static BigInteger TripletChunk(BigInteger T, long segmentStart, long segmentEnd)
        {
            BigInteger count = 0;
            for (long i = segmentStart; i <= segmentEnd; i++)
            {
                var bound2 = _.U_(2, T, new BigInteger(i));
                var step = bound2 - i;
                count += step;
            }

            return count;
        }

        public static long GenQuadruplets(long T, bool write = false)
        {
            long[] U = new long[4];
            long[] i = new long[4];
            long count = 0;

            U[3] = _.U(4, T, 0);
            for (i[3] = 1; i[3] <= U[3]; i[3]++)
            {
                U[2] = _.U(3, T, i[3]);
                for (i[2] = i[3] + 1; i[2] <= U[2]; i[2]++)
                {
                    U[1] = _.U(2, T, i[3] + i[2]);
                    for (i[1] = i[2] + 1; i[1] <= U[1]; i[1]++)
                    {
                        U[0] = _.U(1, T, i[3] + i[2] + i[1]);


                        i[0] = U[0];
                        if (write) Console.WriteLine(string.Join(", ", i));

                        count++;
                    }
                }
            }

            return count;
        }

        //public static void Quadruplets(long T)
        //{
        //    long count = 0;

        //    var bound4 = _.UpperBounds(T, 4, 0);
        //    for (long i = 1; i <= bound4; i++)
        //    {
        //        var bound3 = _.UpperBounds(T, 3, i);
        //        for (long j = i + 1; j <= bound3; j++)
        //        {
        //            var bound2 = _.UpperBounds(T, 2, i + j);

        //            var step = bound2 - j;
        //            count += step;
        //        }
        //    }

        //    Console.WriteLine(count);
        //}

        //public static void TestMethodSextuplets(long T)
        //{
        //    var n = _.nFromT(T);
        //    Console.WriteLine($"n = {n}");

        //    long count = 0;

        //    // Sextuplets
        //    var bounds6 = _.UpperBounds(T, 6, 0);
        //    for (long g = 1; g <= bounds6; g++)
        //    {
        //        // Quintuplets
        //        var bounds5 = _.UpperBounds(T, 5, g);
        //        for (long h = g + 1; h <= bounds5; h++)
        //        {
        //            // Quadruplets
        //            var bounds4 = _.UpperBounds(T, 4, g + h);
        //            for (long i = h + 1; i <= bounds4; i++)
        //            {
        //                // Triplets
        //                var bounds3 = _.UpperBounds(T, 3, g + h + i);
        //                for (long j = i + 1; j <= bounds3; j++)
        //                {
        //                    //Doublets
        //                    var bounds2 = _.UpperBounds(T, 2, g + h + i + j);
        //                    for (long k = j + 1; k <= bounds2; k++)
        //                    {
        //                        var l = T - (g + h + i + j + k);
        //                        count++;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    Console.WriteLine(count);
        //}

        //public static void TestMethod2(long T)
        //{
        //    var n = _.nFromT(T);
        //    Console.WriteLine($"n = {n}");

        //    long count = 0;

        //    // Sextuplets
        //    var bounds6 = _.UpperBounds(T, 6, 0);
        //    for (long g = 1; g <= bounds6; g++)
        //    {
        //        // Qulonguplets
        //        var bounds5 = _.UpperBounds(T, 5, g);
        //        for (long h = g + 1; h <= bounds5; h++)
        //        {
        //            // Quadruplets
        //            var bounds4 = _.UpperBounds(T, 4, g + h);
        //            for (long i = h + 1; i <= bounds4; i++)
        //            {
        //                // Triplets
        //                var bounds3 = _.UpperBounds(T, 3, g + h + i);
        //                for (long j = i + 1; j <= bounds3; j++)
        //                {
        //                    //Doublets
        //                    var bounds2 = _.UpperBounds(T, 2, g + h + i + j);
        //                    count += (bounds2 - j);
        //                }
        //            }
        //        }
        //    }

        //    Console.WriteLine(count);
        //}

        //public static void TestMethod3(long T)
        //{
        //    var n = _.nFromT(T);
        //    Console.WriteLine($"n = {n}");

        //    long count = 0;

        //    // Sextuplets
        //    var bounds6 = _.UpperBounds(T, 6, 0);
        //    for (long g = 1; g <= bounds6; g++)
        //    {
        //        // Qulonguplets
        //        var bounds5 = _.UpperBounds(T, 5, g);
        //        for (long h = g + 1; h <= bounds5; h++)
        //        {
        //            // Quadruplets
        //            var bounds4 = _.UpperBounds(T, 4, g + h);
        //            for (long i = h + 1; i <= bounds4; i++)
        //            {
        //                Console.WriteLine();

        //                // Triplets
        //                var bounds3 = _.UpperBounds(T, 3, g + h + i);
        //                for (long j = i + 1; j <= bounds3; j++)
        //                {
        //                    //Doublets
        //                    var bounds2 = _.UpperBounds(T, 2, g + h + i + j);

        //                    var step = bounds2 - j;
        //                    count += step;

        //                    Console.WriteLine($"{g}, {h}, {i}, {j}: {step} % {j % 3}");
        //                }
        //            }
        //        }
        //    }

        //    Console.WriteLine(count);
        //}
    }
}
