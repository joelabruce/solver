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
            Stopwatch sw = new Stopwatch();

            // Time how long the algorithm takes to complete and then print results
            sw.Start();
            // Anything larger will result in numeric overflow
            //var x = TripletsAsync(64, 10000000000, 0).Result;     // Takes about 15-20 seconds
            //var x = TripletsAsync(14, 1000000000, 0).Result;
            //var x = TripletsAsync(1, 1000000000, 0).Result;
            //var x = TripletsAsync(16, 218, 0).Result;
            sw.Stop();

            //Console.WriteLine($"{x} in {sw.ElapsedMilliseconds}ms");

            //return;

            for (int T = 218; T <= 300; T++)
            {
                //Console.Clear();
                //Console.WriteLine($"{SumGenerator.RecursiveTuple(4, T, U, i, 0)}");
                //SumGenerator.RecursiveTuple(5, T, U, i, 0, (indices) => Console.WriteLine(string.Join(", ", indices)));
                //SumGenerator.UniverseGenerator(T, (indices) => Console.WriteLine(string.Join(", ", indices)));
                var n = SumGenerator.nFromT(T);
                
                Console.Write($"{T},,{n},{Math.Pow(2,n)},");
                SumGenerator.UniverseGenerator(T, null, (d, solutions) =>
                {
                    Console.Write($",{solutions}");
                });

                //Console.ReadLine();
                Console.WriteLine();
            }

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

        //public static void TripletCheck()
        //{
        //    long T = 122;
        //    var ub = SumGenerator.UpperBounds(T, 3, 0);
        //    var count = 0;
        //    for (long i = 1; i <= ub; i++)
        //    {
        //        var pairs = SumGenerator.GenDoublets(T, i);
        //        var thisCount = pairs.Count();
        //        count += thisCount;
        //        Console.WriteLine($"Total pairs for head of {i} = {thisCount}");
        //        foreach (var pair in pairs)
        //        {
        //            Console.WriteLine($"{i}, {pair}");
        //        }
        //    }

        //    Console.WriteLine(count);
        //}

        public static void OperandTest()
        {
            Operand[] operands = new Operand[5];
            operands[0] = 5f._("a") + 10f._("b"); 
            operands[1] = 17f._("c");
            operands[2] = operands[0] * operands[1];

            Console.WriteLine(operands[2]);
            Console.WriteLine(operands[2].ToExpression());
        }

        public static async Task<long> TripletsAsync(long threads, long T, long offset)
        {
            var bound3 = SumGenerator.UpperBounds(T, 3, 0);

            Segment[] segments = SumGenerator.FrontLoadedThreadSegmentGenerator(threads, bound3);
            var segmentCount = segments.Length;


            Task<long>[] tasks = new Task<long>[segmentCount];
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

            return counts.Sum();
        }

        public static long TripletChunk(long T, long segmentStart, long segmentEnd)
        {
            long count = 0;
            for (long i = segmentStart; i <= segmentEnd; i++)
            {
                var bound2 = SumGenerator.UpperBounds(T, 2, i);
                var step = bound2 - i;
                count += step;
            }

            return count;
        }

        public static long GenTriplets(long T, bool write = false)
        {
            long[] U = new long[3];
            long[] i = new long[3];
            long count = 0;

            U[2] = SumGenerator.U(3, T, 0);
            for (i[0] = 1; i[0] <= U[2]; i[0]++)
            {
                U[1] = SumGenerator.U(2, T, i[0]);
                for (i[1] = i[0] + 1; i[1] <= U[1]; i[1]++)
                {
                    U[0] = SumGenerator.U(1, T, i[0] + i[1]);
                    i[2] = U[0];
                    if (write) Console.WriteLine(string.Join(", ", i));

                    count++;
                }
            }

            return count;
        }

        public static long GenQuadruplets(long T, bool write = false)
        {
            long[] U = new long[4];
            long[] i = new long[4];
            long count = 0;

            U[3] = SumGenerator.U(4, T, 0);
            for (i[3] = 1; i[3] <= U[3]; i[3]++)
            {
                U[2] = SumGenerator.U(3, T, i[3]);
                for (i[2] = i[3] + 1; i[2] <= U[2]; i[2]++)
                {
                    U[1] = SumGenerator.U(2, T, i[3] + i[2]);
                    for (i[1] = i[2] + 1; i[1] <= U[1]; i[1]++)
                    {
                        U[0] = SumGenerator.U(1, T, i[3] + i[2] + i[1]);


                        i[0] = U[0];
                        if (write) Console.WriteLine(string.Join(", ", i));

                        count++;
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tupleSize"></param>
        /// <param name="T">Target sum.</param>
        /// <param name="U">Upperbound for a given dimension.</param>
        /// <param name="i">Index for a given dimension.</param>
        /// <param name="lowerbound">Accumulated lower bounds.</param>
        /// <param name="write">Outputs to console if true.</param>
        /// <returns></returns>
        //public static long RecursiveTuple(long tupleSize, long T, long[] U, long[] i, long lowerbound = 0, bool write = false)
        //{
        //    long nextLayer = tupleSize - 1;
        //    U[nextLayer] = SumGenerator.U(tupleSize, T, lowerbound);
        //    long count = 0;

        //    if (nextLayer == 0)
        //    {
        //        i[0] = U[0];
        //        if (write) Console.WriteLine(string.Join(", ", i));
        //        return 1;
        //    }
        //    else
        //    {
        //        for (i[nextLayer] = i[tupleSize] + 1; i[nextLayer] <= U[nextLayer]; i[nextLayer]++)
        //        {
        //            count += RecursiveTuple(tupleSize - 1, T, U, i, lowerbound + i[nextLayer], write);
        //        }
        //    }

        //    return count;
        //}

        public static void Quadruplets(long T)
        {
            long count = 0;

            var bound4 = SumGenerator.UpperBounds(T, 4, 0);
            for (long i = 1; i <= bound4; i++)
            {
                var bound3 = SumGenerator.UpperBounds(T, 3, i);
                for (long j = i + 1; j <= bound3; j++)
                {
                    var bound2 = SumGenerator.UpperBounds(T, 2, i + j);

                    var step = bound2 - j;
                    count += step;
                }
            }

            Console.WriteLine(count);
        }

        public static void TestMethodSextuplets(long T)
        {
            var n = SumGenerator.nFromT(T);
            Console.WriteLine($"n = {n}");

            long count = 0;

            // Sextuplets
            var bounds6 = SumGenerator.UpperBounds(T, 6, 0);
            for (long g = 1; g <= bounds6; g++)
            {
                // Quintuplets
                var bounds5 = SumGenerator.UpperBounds(T, 5, g);
                for (long h = g + 1; h <= bounds5; h++)
                {
                    // Quadruplets
                    var bounds4 = SumGenerator.UpperBounds(T, 4, g + h);
                    for (long i = h + 1; i <= bounds4; i++)
                    {
                        // Triplets
                        var bounds3 = SumGenerator.UpperBounds(T, 3, g + h + i);
                        for (long j = i + 1; j <= bounds3; j++)
                        {
                            //Doublets
                            var bounds2 = SumGenerator.UpperBounds(T, 2, g + h + i + j);
                            for (long k = j + 1; k <= bounds2; k++)
                            {
                                var l = T - (g + h + i + j + k);
                                count++;
                            }
                        }
                    }
                }
            }

            Console.WriteLine(count);
        }

        public static void TestMethod2(long T)
        {
            var n = SumGenerator.nFromT(T);
            Console.WriteLine($"n = {n}");

            long count = 0;

            // Sextuplets
            var bounds6 = SumGenerator.UpperBounds(T, 6, 0);
            for (long g = 1; g <= bounds6; g++)
            {
                // Qulonguplets
                var bounds5 = SumGenerator.UpperBounds(T, 5, g);
                for (long h = g + 1; h <= bounds5; h++)
                {
                    // Quadruplets
                    var bounds4 = SumGenerator.UpperBounds(T, 4, g + h);
                    for (long i = h + 1; i <= bounds4; i++)
                    {
                        // Triplets
                        var bounds3 = SumGenerator.UpperBounds(T, 3, g + h + i);
                        for (long j = i + 1; j <= bounds3; j++)
                        {
                            //Doublets
                            var bounds2 = SumGenerator.UpperBounds(T, 2, g + h + i + j);
                            count += (bounds2 - j);
                        }
                    }
                }
            }

            Console.WriteLine(count);
        }

        public static void TestMethod3(long T)
        {
            var n = SumGenerator.nFromT(T);
            Console.WriteLine($"n = {n}");

            long count = 0;

            // Sextuplets
            var bounds6 = SumGenerator.UpperBounds(T, 6, 0);
            for (long g = 1; g <= bounds6; g++)
            {
                // Qulonguplets
                var bounds5 = SumGenerator.UpperBounds(T, 5, g);
                for (long h = g + 1; h <= bounds5; h++)
                {
                    // Quadruplets
                    var bounds4 = SumGenerator.UpperBounds(T, 4, g + h);
                    for (long i = h + 1; i <= bounds4; i++)
                    {
                        Console.WriteLine();

                        // Triplets
                        var bounds3 = SumGenerator.UpperBounds(T, 3, g + h + i);
                        for (long j = i + 1; j <= bounds3; j++)
                        {
                            //Doublets
                            var bounds2 = SumGenerator.UpperBounds(T, 2, g + h + i + j);

                            var step = bounds2 - j;
                            count += step;

                            Console.WriteLine($"{g}, {h}, {i}, {j}: {step} % {j % 3}");
                        }
                    }
                }
            }

            Console.WriteLine(count);
        }
    }
}
