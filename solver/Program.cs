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
            const long desiredTaskCount = 4; // 3 to 4 segments seems to be the magic number for performance

            long start = 6;// (long) Math.Pow(10, 10);
            long end = 117;
            
            var tms = Timed(() =>
            {
                for (long task = start; task <= start + end; task++)
                {
                    var T = task;

                    var testChoice = 2;
                    switch (testChoice)
                    {
                        case 1: // Generate strict triplets of T using dynamic-programming vs fast algorithm
                            var (ms, r) = TimedFunc(() => TripletsAsync(desiredTaskCount, T).Result);
                            var (msf, rf) = TimedFunc(() => _.StrictPartitionTriplets(T));
                            Console.WriteLine($"{r} in {ms}ms");
                            Console.WriteLine($"{rf} in {msf}ms (f)");
                            break;
                        case 2: // Generate quadruplets using recursive algorithm, to try to find the next pattern
                            BigInteger[] U = new BigInteger[5];
                            BigInteger[] i = new BigInteger[5];
                            var counts = _.RecursiveTupleBI(4, T, U, i, 0, (indices) =>
                            {
                                Console.WriteLine($"{string.Join(", ", indices)}");
                            });
                            break;
                        default:
                            break;
                    }
                }
            });
            
            Console.WriteLine($"Total ms: {tms}, Avg: {tms / end}");
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

        public static BigInteger GenQuadruplets(BigInteger T, Action<BigInteger[]> processQuads)
        {
            BigInteger[] U = new BigInteger[4];
            BigInteger[] i = new BigInteger[4];
            BigInteger count = 0;

            U[3] = _.U_(4, T, 0);
            for (i[0] = 1; i[0] <= U[3]; i[0]++)
            {
                Console.WriteLine($"{i[0]},");
                U[2] = _.U_(3, T, i[0]);
                for (i[1] = i[0] + 1; i[1] <= U[2]; i[1]++)
                {
                    Console.WriteLine($"   {i[1]},");
                    U[1] = _.U_(2, T, i[0] + i[1]);
                    for (i[2] = i[1] + 1; i[2] <= U[1]; i[2]++)
                    {
                        U[0] = _.U_(1, T, i[0] + i[1] + i[2]);

                        i[3] = U[0];
                        //processQuads.Invoke(i);
                        Console.WriteLine($"      {string.Join(", ", i.Skip(2))}");

                        count++;
                    }
                }
            }

            return count;
        }

        public static BigInteger GenTriplets(BigInteger T, Action<BigInteger[]> processQuads)
        {
            BigInteger[] U = new BigInteger[4];
            BigInteger[] i = new BigInteger[4];
            BigInteger count = 0;

            U[2] = _.U_(3, T, 0);
            for (i[0] = 1; i[0] <= U[2]; i[0]++)
            {
                Console.WriteLine($"{i[0]},");
                U[1] = _.U_(2, T, i[0]);
                for (i[1] = i[0] + 1; i[1] <= U[1]; i[1]++)
                {
                    U[0] = _.U_(1, T, i[0] + i[1]);                    
                    i[2] = U[0];
                    
                    Console.WriteLine($"      {string.Join(", ", i.Skip(1))}");
                    count++;
                }
            }

            return count;
        }
    }
}
