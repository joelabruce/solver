using NUnit.Framework;
using solver;
using solver.PMath;

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
            // Segments actual quantity should be the lesser of desired tasks or upperBound
            var segments1 = _.FrontLoadedThreadSegmentGenerator(7, 6);
            Assert.AreEqual(6, segments1.Length);

            // Segments actual quantity when upperBound and desiredThreadCount are equal
            var segments2 = _.FrontLoadedThreadSegmentGenerator(7, 7);
            Assert.AreEqual(7, segments2.Length);

            // Should have 4 equal sized segments
            var segments3 = _.FrontLoadedThreadSegmentGenerator(4, 20);
            Assert.AreEqual(1, segments3[0].Start);
            Assert.AreEqual(5, segments3[0].End);
            Assert.AreEqual(6, segments3[1].Start);
            Assert.AreEqual(10, segments3[1].End);
            Assert.AreEqual(11, segments3[2].Start);
            Assert.AreEqual(15, segments3[2].End);
            Assert.AreEqual(16, segments3[3].Start);
            Assert.AreEqual(20, segments3[3].End);

            // Should have 4 segments, and the first 1 should have an extra
            var segments4 = _.FrontLoadedThreadSegmentGenerator(4, 21);
            Assert.AreEqual(1, segments4[0].Start);
            Assert.AreEqual(6, segments4[0].End);
            Assert.AreEqual(7, segments4[1].Start);
            Assert.AreEqual(11, segments4[1].End);
            Assert.AreEqual(12, segments4[2].Start);
            Assert.AreEqual(16, segments4[2].End);
            Assert.AreEqual(17, segments4[3].Start);
            Assert.AreEqual(21, segments4[3].End);

            // Should have 4 segments, and the first 2 should have an extra
            var segments5 = _.FrontLoadedThreadSegmentGenerator(4, 22);
            Assert.AreEqual(1, segments5[0].Start);
            Assert.AreEqual(6, segments5[0].End);
            Assert.AreEqual(7, segments5[1].Start);
            Assert.AreEqual(12, segments5[1].End);
            Assert.AreEqual(13, segments5[2].Start);
            Assert.AreEqual(17, segments5[2].End);
            Assert.AreEqual(18, segments5[3].Start);
            Assert.AreEqual(22, segments5[3].End);

            // Should have 4 segments, and the first 3 have 1 more than the last
            var segments6 = _.FrontLoadedThreadSegmentGenerator(4, 23);
            Assert.AreEqual(1, segments6[0].Start);
            Assert.AreEqual(6, segments6[0].End);
            Assert.AreEqual(7, segments6[1].Start);
            Assert.AreEqual(12, segments6[1].End);
            Assert.AreEqual(13, segments6[2].Start);
            Assert.AreEqual(18, segments6[2].End);
            Assert.AreEqual(19, segments6[3].Start);
            Assert.AreEqual(23, segments6[3].End);

            // Should have 4 equal segments again
            var segments7 = _.FrontLoadedThreadSegmentGenerator(4, 24);
            Assert.AreEqual(1, segments7[0].Start);
            Assert.AreEqual(6, segments7[0].End);
            Assert.AreEqual(7, segments7[1].Start);
            Assert.AreEqual(12, segments7[1].End);
            Assert.AreEqual(13, segments7[2].Start);
            Assert.AreEqual(18, segments7[2].End);
            Assert.AreEqual(19, segments7[3].Start);
            Assert.AreEqual(24, segments7[3].End);
        }

        [Test]
        public void Test2()
        {
            Operand[] operands = new Operand[5];
            operands[0] = 5f._("a") + 10f._("b");
            operands[1] = 17f._("c");
            operands[2] = operands[0] * operands[1];

            Assert.AreEqual("((a+b)*c)", operands[2].ToExpression());
        }
    }
}