using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jaws.Data;
using System.Linq;
using System.Diagnostics;

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
        public void FlatSplitTest()
        {
            var start = new TestNode("start");
            var split = new TestNode("split");
            var tree = new CircularQuadTree<TestNode>(start, split);
            tree.Split(split);

            var current = start;
            for (int i = 0; i < 3; i++)
                current = tree.GetNeighbours(current, 0, 1).First();
            Assert.AreEqual(current, start);
        }

        [TestMethod]
        public void SplitTest()
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
            tree.Split(split);

            var current = start;
            for (int i = 0; i < 7; i++)
                current = tree.GetNeighbours(current, 0, 1).First();

           Assert.AreEqual(start, current);
        }

        [TestMethod]
        public void GetNeighboursTest()
        {
            var start = new TestNode();
            var tree = new CircularQuadTree<TestNode>(
                start,
                new TestNode(),
                new TestNode(),
                new TestNode(),
                new TestNode(),
                new TestNode());

            Assert.AreNotEqual(start, tree.GetNeighbours(start, 1, 0).First());
        }

        [TestMethod]
        public void ReplaceTest()
        {
            var start = new TestNode("start");
            var split = new TestNode("split");
            var tree = new CircularQuadTree<TestNode>(
                start,
                split,
                new TestNode("1"),
                new TestNode("2"),
                new TestNode("3"),
                new TestNode("4"));
            var current = start;
            for (int i = 0; i < 6; i++)
                current = tree.GetNeighbours(current, 0, 1).First();
            Assert.AreEqual(start, current);

            var newnodes = tree.Split(split);

            current = start;
            for (int i = 0; i < 7; i++)
                current = tree.GetNeighbours(current, 0, 1).First();
            Assert.AreEqual(start, current);

            tree.Replace((TestNode)newnodes[0], 0, split);

            current = start;
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

            public override string ToString()
            {
                return Name;
            }

            IQuadNode[] IQuadNode.Split()
            {
                return new IQuadNode[] { new TestNode(Name + "-topleft"), new TestNode(Name + "-topright"), new TestNode(Name + "-bottomleft"), new TestNode(Name + "-bottomright") };
            }
        }
    }
}
