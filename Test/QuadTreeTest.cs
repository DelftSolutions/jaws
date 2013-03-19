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
                current = tree.GetNeighbours(current, 0, 1).First();

            Assert.AreEqual(start, current);
        }

        [TestMethod]
        public void SplitTest()
        {
            var start = new TestNode();
            var split = new TestNode();
            var tree = new CircularQuadTree<TestNode>(
                start,
                split,
                new TestNode(),
                new TestNode(),
                new TestNode(),
                new TestNode());
            tree.Split(split);

            var current = start;
            for (int i = 0; i < 7; i++)
                current = tree.GetNeighbours(current, 0, 1).First();

            Assert.AreEqual(start, current);
        }

        [TestMethod]
        public void ReplaceTest()
        {
            var start = new TestNode("start");
            var split = new TestNode("split");
            var tree = new CircularQuadTree<TestNode>(
                start,
                split,
                new TestNode(),
                new TestNode(),
                new TestNode(),
                new TestNode());
            var newnodes = tree.Split(split);
            tree.Replace((TestNode)newnodes[0], 0, split);

            var current = start;
            for (int i = 0; i < 6; i++)
                current = tree.GetNeighbours(current, 0, 1).First();

            Assert.AreEqual(start, current);
        }

        class TestNode : IQuadNode
        {
            public TestNode() { }
            public TestNode(String name)
            {
                Name = name;
            }

            public String Name { get; set; }

            IQuadNode[] IQuadNode.Split()
            {
                return new IQuadNode[] { new TestNode(Name + "-topleft"), new TestNode(Name + "-topright"), new TestNode(Name + "-bottomright"), new TestNode(Name + "-bottomleft") };
            }
        }
    }
}
