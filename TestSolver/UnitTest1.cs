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
            var segments1 = _.FrontLoadedThreadSegmentGenerator(7, 6);
            var segments2 = _.FrontLoadedThreadSegmentGenerator(7, 7);
            var segments3 = _.FrontLoadedThreadSegmentGenerator(5, 20);
            var segments4 = _.FrontLoadedThreadSegmentGenerator(5, 21);
            var segments5 = _.FrontLoadedThreadSegmentGenerator(5, 22);
            var segments6 = _.FrontLoadedThreadSegmentGenerator(5, 23);
            var segments7 = _.FrontLoadedThreadSegmentGenerator(5, 24);
            var segments8 = _.FrontLoadedThreadSegmentGenerator(5, 25);
            var segments9 = _.FrontLoadedThreadSegmentGenerator(5, 26);
        }
    }
}