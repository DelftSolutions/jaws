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
        public void OrientationTest()
        {
            var start = new TestNode();
            var tree = new CircularQuadTree<TestNode>(start);

            var current = start;
            for (int i = 0; i < 1; i++)
                current = tree.GetNeighbours(current, 0, 1).First();

            Assert.AreEqual(start, current);
        }

        [TestMethod]
        public void SplitTest()
        {
            var start = new TestNode("start");
            var tree = new CircularQuadTree<TestNode>(start);
            var res = tree.Split(start);

            var current = (TestNode)res[0];
            for (int i = 0; i < 2; i++)
                current = tree.GetNeighbours(current, 0, 1).First();
            Assert.AreEqual((TestNode)res[0], current);
        }

        [TestMethod]
        public void GetNeighboursTest()
        {
            var start = new TestNode();
            var tree = new CircularQuadTree<TestNode>(start);

            Assert.AreEqual(start, tree.GetNeighbours(start, 1, 0).First());
        }

        [TestMethod]
        public void ReplaceTest()
        {
            var start = new TestNode("start");
            var tree = new CircularQuadTree<TestNode>(start);
            var current = start;
            for (int i = 0; i < 1; i++)
                current = tree.GetNeighbours(current, 0, 1).First();
            Assert.AreEqual(start, current);

            var newnodes = tree.Split(start);

            current = (TestNode)newnodes[0];
            for (int i = 0; i < 2; i++)
                current = tree.GetNeighbours(current, 0, 1).First();
            Assert.AreEqual((TestNode)newnodes[0], current);

            tree.Replace((TestNode)newnodes[0], 0, start);

            current = start;
            for (int i = 0; i < 1; i++)
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
