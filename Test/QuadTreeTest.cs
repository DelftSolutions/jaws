using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jaws.Data;
using System.Linq;

namespace Test
{
    [TestClass]
    public class QuadTreeTest
    {
        [TestMethod]
        public void InitTest()
        {
            var tree = new CircularQuadTree<TestNode>(
                new TestNode(),
                new TestNode(),
                new TestNode(),
                new TestNode(),
                new TestNode(),
                new TestNode());
        }

        [TestMethod]
        public void OrientationTest()
        {
            var start = new TestNode();
            var tree = new CircularQuadTree<TestNode>(
                start,
                new TestNode(),
                new TestNode(),
                new TestNode(),
                new TestNode(),
                new TestNode());

            var current = start;
            for (int i = 0; i < 6; i++)
                current = tree.GetNeighboursUp(current).First();

            Assert.AreEqual(start, current);
        }

        class TestNode : IQuadNode
        {

            Tuple<IQuadNode, IQuadNode, IQuadNode, IQuadNode> IQuadNode.Split()
            {
                throw new NotImplementedException();
            }
        }
    }
}
