using NUnit.Framework;
using solver;

namespace TestSolver
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var segments1 = SumGenerator.FrontLoadedThreadSegmentGenerator(7, 6);
            var segments2 = SumGenerator.FrontLoadedThreadSegmentGenerator(7, 7);
            var segments3 = SumGenerator.FrontLoadedThreadSegmentGenerator(5, 20);
            var segments4 = SumGenerator.FrontLoadedThreadSegmentGenerator(5, 21);
            var segments5 = SumGenerator.FrontLoadedThreadSegmentGenerator(5, 22);
            var segments6 = SumGenerator.FrontLoadedThreadSegmentGenerator(5, 23);
            var segments7 = SumGenerator.FrontLoadedThreadSegmentGenerator(5, 24);
            var segments8 = SumGenerator.FrontLoadedThreadSegmentGenerator(5, 25);
            var segments9 = SumGenerator.FrontLoadedThreadSegmentGenerator(5, 26);
        }
    }
}