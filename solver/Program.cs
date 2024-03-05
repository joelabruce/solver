using System;

namespace solver
{
    class Program
    {
        static void Main(string[] args)
        {
            // Example of all possible quadruplets for a given T.
            int T = 34;

            var n = SumGenerator.nFromT(T);
            Console.WriteLine($"n = {n}");

            // Quadruplets
            var bounds4 = SumGenerator.UpperBounds(T, 4, 0);
            for (int i = 1; i <= bounds4; i++)
            {
                // Triplets
                var bounds3 = SumGenerator.UpperBounds(T, 3, i);
                for (int j = i + 1; j <= bounds3; j++)
                {
                    //Doublets
                    var bounds2 = SumGenerator.UpperBounds(T, 2, i + j);
                    for (int k = j + 1; k <= bounds2; k++)
                    {
                        var l = T - (i + j + k);
                        Console.WriteLine($"{i}, {j}, {k}, {l}");
                    }
                }
            }
        }
    }
}
