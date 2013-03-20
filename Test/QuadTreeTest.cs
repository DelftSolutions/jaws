using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jaws.Data;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

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
        public void SplitReplaceTest()
        {
            var start = new TestNode();
            var tree = new CircularQuadTree<TestNode>(start);
            int times = 200;
            TestNode[] nodes = new TestNode[times];
            nodes[0] = start;
            for (int i = 1; i < times; i++)
            {
                var res = tree.Split(nodes[i - 1]);
                nodes[i] = (TestNode)res[i % 4];
            }

            Assert.AreEqual(3 * (times - 1) + 1, tree.GetArea(nodes[times - 1], 0).Count(), "Tree should contain a different amount of children after these splits");

            for (int i = times - 1; i > 0; i--)
            {
                tree.Replace(nodes[i], i - 1, nodes[i - 1]);
                
            }
            Assert.AreEqual(1, tree.GetArea(nodes[0], 0).Count(), "Tree should contain a different amount of children after these merges");
        }

        [TestMethod]
        public void SplitAllTest()
        {
            var start = new TestNode("");
            var tree = new CircularQuadTree<TestNode>(start);
            Queue<TestNode> q = new Queue<TestNode>();
            q.Enqueue(start);

            int width = 1;
            for (int round = 0; round < 4; round++)
            {
                for (int i = 0; i < (width * width); i++)
                {
                    var s = q.Dequeue();
                    var childs = tree.Split(s);
                    foreach (var child in childs)
                        q.Enqueue((TestNode)child);
                }

                width *= 2;

                start = q.Peek();
                var cur = start;
                for (int i = 0; i < width; i++)
                    cur = tree.GetNeighbours(cur, 1, 0).First();
                Assert.AreEqual(start, cur);
            }
        }

        [TestMethod]
        public void GetAreaTest()
        {
            var start = new TestNode("start");
            var tree = new CircularQuadTree<TestNode>(start);
            Assert.AreEqual(1, tree.GetArea(start, 0).Count(), "There should be one element in this collection");
            var res = tree.Split(start);
            Assert.AreEqual(4, tree.GetArea((TestNode)res[0], 0).Count(), "After a split this collection should contain 4 elements");
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
                if (Name != null)
                    return new IQuadNode[] { new TestNode(Name + "-topleft"), new TestNode(Name + "-topright"), new TestNode(Name + "-bottomleft"), new TestNode(Name + "-bottomright") };
                else
                    return new IQuadNode[] { new TestNode(), new TestNode(), new TestNode(), new TestNode() };
            }
        }
    }
}
