﻿using System;
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
            const long desiredTaskCount = 4; // 3 to 4 segments seems to be the magic number for performance

            //long start = 1;// (long) Math.Pow(10, 10);
            //long tasks = 50;

            var exploratory = false;
            if (exploratory)
            {
                for (BigInteger T = 1; T < 48; T++)
                {
                    BigInteger result = 0;
                    BigInteger nmod3 = (-T).Mod(3);
                    BigInteger mod3 = T.Mod(3);
                    var ub = (T + (3 - T).Mod(3)) / 3;

                    Console.WriteLine();
                    Console.WriteLine($"{T} |{ub}| +");
                    for (long i = 2; i <= ub; i++)
                    {

                        //long term1 = 3 * i - 2, i1 = (long)Math.Floor(term1 / 2d) - 2;
                        //long term2 = 3 * i - 1, i2 = (long)Math.Floor(term2 / 2d) - 2;
                        //long term3 = 3 * i - 0, i3 = (long)Math.Floor(term3 / 2d) - 2;
                        //Console.WriteLine($"{term1,4}: {i1,4}, {term2,4}: {i2,4}, {term3,4}: {i3,4}");

                        var term = 3 * i - (nmod3);
                        //var j = (BigInteger)Math.Floor((double)term / 2) - 2;
                        var j = term / 2 - 2;
                        Console.WriteLine($"{j}");
                        result += j;
                    }

                    Console.WriteLine($"={result}");
                }
                return;
            }

            Console.WriteLine("0) Generate Universe of solutions");
            Console.WriteLine("1) Test and time strict triplet implementations");
            Console.WriteLine("2) Test Generating Quadruplets");
            Console.WriteLine("3) Test Triplet patterns");
            Console.WriteLine("4) Test Quadruplet patterns");
            Console.WriteLine("5) Test Quintuplet patterns");
            Console.WriteLine("6) Joel Series");
            var testChoice = Console.ReadLine() + ",1,20";

            var (choice, start, tasks) = testChoice.Split(",");
            var tms = Timed(() =>
            {
                for (long task = start; task <= start + tasks; task++)
                {
                    var T = task;

                    switch (choice)
                    {
                        case 0:
                            _.UniverseGenerator(T, (indices) =>
                            {
                                Console.WriteLine(string.Join(", ", indices));
                            });
                            break;
                        case 1: // Generate strict triplets of T using dynamic-programming vs fast algorithm
                            var (ms, r) = TimedFunc(() => TripletsAsync(desiredTaskCount, T).Result);
                            var (msf, rf) = TimedFunc(() => _.StrictPartitionTriplets(T));
                            var (msf2, rf2) = TimedFunc(() => _.PhasedPentagonalTriplets(T));
                            var (msf3, rf3) = TimedFunc(() => _.SimpleTriplets(T));
                            var (msf4, rf4) = TimedFunc(() => _.StrictPartitionTripletsAdd(T));
                            Console.WriteLine(T);
                            Console.WriteLine($"{r} in {ms}ms");
                            Console.WriteLine($"{rf} in {msf}ms (f)");
                            //Console.WriteLine($"{rf2} in {msf2}ms (f2)");
                            Console.WriteLine($"{rf3} in {msf3}ms (f3)");
                            Console.WriteLine($"{rf4} in {msf4}ms (f4)");

                            //if (r != rf2 || r != rf2 || r != rf3 || r != rf4)
                            //    Console.WriteLine($"Exception found! {T}");

                            Console.WriteLine();
                            break;
                        case 2: // Generate quadruplets using recursive algorithm, to try to find the next pattern
                            BigInteger[] U = new BigInteger[5];
                            BigInteger[] i = new BigInteger[5];
                            var counts = _.RecursiveTupleBI(4, T, U, i, 0, (indices) =>
                            {
                                Console.WriteLine($"{string.Join(", ", indices)}");
                            });
                            break;
                        case 3:
                            Console.WriteLine(GenTriplets(T, true));
                            break;
                        case 4:
                            Console.WriteLine(GenQuadruplets(T, true));
                            break;
                        case 5:
                            Console.WriteLine(GenQuintuplets(T, true));
                            break;
                        case 6:
                            //Console.Write($"{_.JoelSeries(T)}, ");
                            Console.Write($"{_.JoelSeries(T)}, ");
                            break;
                        default:
                            break;
                    }
                }
            });

            Console.WriteLine();
            Console.WriteLine($"Total ms: {tms}, Avg: {tms / tasks}");
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

        public static void GenerateAllSolutionsForT(long start, long end)
        {
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

        public static async Task<BigInteger> TripletsAsync(long threads, BigInteger T)
        {
            var bound3 = _.U_(3, T, 0);

            if (bound3 > long.MaxValue)
            {
                Console.WriteLine($"{long.MaxValue} $ {bound3.ToString()}");
                Console.WriteLine($"{bound3 > long.MaxValue}");
            }

            Segment[] segments = _.FrontLoadedThreadSegmentGenerator(threads, (long)bound3);
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
            foreach (var count in counts)
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

        public static BigInteger GenQuintuplets(BigInteger T, bool includeBreakdown = false)
        {
            BigInteger[] U = new BigInteger[5];
            BigInteger[] i = new BigInteger[5];
            BigInteger count = 0;
            BigInteger quadsForQuints = 0;

            Console.WriteLine();
            if (includeBreakdown) Console.Write($"{T}, % {T % 5} ");
            Console.WriteLine();

            U[4] = _.U_(5, T, 0);
            for (i[0] = 1; i[0] <= U[4]; i[0]++)
            {
                quadsForQuints = 0;

                BigInteger guess = _.PhasedPentagonalTriplets(T - i[0]);

                if (includeBreakdown) Console.Write($"{i[0]} -> ({guess}) ");
                U[3] = _.U_(4, T, i[0]);
                for (i[1] = i[0] + 1; i[1] <= U[3]; i[1]++)
                {
                    U[2] = _.U_(3, T, i[0] + i[1]);
                    for (i[2] = i[1] + 1; i[2] <= U[2]; i[2]++)
                    {
                        U[1] = _.U_(2, T, i[0] + i[1] + i[2]);
                        for (i[3] = i[2] + 1; i[3] <= U[1]; i[3]++)
                        {
                            U[0] = _.U_(1, T, i[0] + i[1] + i[2] + i[3]);
                            i[4] = U[0];

                            count++;
                            quadsForQuints++;
                        }
                    }
                }

                if (includeBreakdown) Console.WriteLine(quadsForQuints);
            }

            return count;
        }

        public static BigInteger GenQuadruplets(BigInteger T, bool includeBreakdown = false)
        {
            BigInteger[] U = new BigInteger[4];
            BigInteger[] i = new BigInteger[4];
            BigInteger count = 0;
            BigInteger tripsForQuads = 0;
            BigInteger pairsForTrips = 0;

            Console.WriteLine();
            if (includeBreakdown) Console.Write($"{T}, % {T % 4} ");
            Console.WriteLine($"({_.PhasedHexagonalQuadruplets(T)}) [{_.TestQuadrupletsSolver(T)}]");

            U[3] = _.U_(4, T, 0);
            for (i[0] = 1; i[0] <= U[3]; i[0]++)
            {
                tripsForQuads = 0;
                if (includeBreakdown) Console.Write($"{i[0]} -> ");
                if (includeBreakdown) Console.WriteLine();


                U[2] = _.U_(3, T, i[0]);
                for (i[1] = i[0] + 1; i[1] <= U[2]; i[1]++)
                {
                    pairsForTrips = 0;
                    U[1] = _.U_(2, T, i[0] + i[1]);
                    for (i[2] = i[1] + 1; i[2] <= U[1]; i[2]++)
                    {
                        U[0] = _.U_(1, T, i[0] + i[1] + i[2]);
                        i[3] = U[0];

                        Console.WriteLine($"   {string.Join(", ", i.Skip(1))}");

                        count++;
                        tripsForQuads++;
                        pairsForTrips++;
                    }
                    Console.WriteLine($"     {pairsForTrips}");
                }

                if (includeBreakdown) Console.WriteLine(tripsForQuads);
            }

            return count;
        }

        public static BigInteger GenTriplets(BigInteger T, bool includeBreakdown = false)
        {
            BigInteger[] U = new BigInteger[4];
            BigInteger[] i = new BigInteger[4];
            BigInteger count = 0;
            BigInteger pairsForTriplets = 0;

            if (includeBreakdown) Console.Write($"{T}, % {T % 3} ");

            //if (T % 6 == 2)
            {
                Console.WriteLine($"({_.PhasedPentagonalTriplets(T)})");
            }

            U[2] = _.U_(3, T, 0);
            for (i[0] = 1; i[0] <= U[2]; i[0]++)
            {
                pairsForTriplets = 0;
                if (includeBreakdown) Console.WriteLine($"{i[0]} ->");
                U[1] = _.U_(2, T, i[0]);
                for (i[1] = i[0] + 1; i[1] <= U[1]; i[1]++)
                {
                    U[0] = _.U_(1, T, i[0] + i[1]);
                    i[2] = U[0];

                    count++;
                    pairsForTriplets++;
                    //Console.WriteLine($"   {string.Join(", ", i.Skip(1))}");
                }

                if (includeBreakdown) Console.WriteLine($"   {pairsForTriplets}");
            }

            return count;
        }
    }
}

public static class Extensions
{
    public static void Deconstruct(this string[] prompts, out int choice, out long start, out long quantity)
    {
        int.TryParse(prompts[0], out choice);
        long.TryParse(prompts[1], out start);
        long.TryParse(prompts[2], out quantity);
    }
}