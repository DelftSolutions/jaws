using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jaws.Data
{
    /// <summary>
    /// Stores nodes on a planetary quadtree
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CircularQuadTree<T> : IEnumerable<T> where T: IQuadNode
    {
        protected Dictionary<T, CircularQuadNode> nodes;

        /// <summary>
        /// Creates a new CircularQuadTree (3d representation)
        /// </summary>
        public CircularQuadTree(T face1, T face2, T face3, T face4, T face5, T face6)
        {
            var f1 = new CircularQuadNode();
            var f2 = new CircularQuadNode();
            var f3 = new CircularQuadNode();
            var f4 = new CircularQuadNode();
            var f5 = new CircularQuadNode();
            var f6 = new CircularQuadNode();

            f1.Value = face1;
            f2.Value = face2;
            f3.Value = face3;
            f4.Value = face4;
            f5.Value = face5;
            f6.Value = face6;

            f1.NeighbourUp = f2;
            f2.NeighbourUp = f3;
            f3.NeighbourUp = f4;
            f4.NeighbourUp = f5;
            f5.NeighbourUp = f6;
            f6.NeighbourUp = f1;

            f1.NeighbourDown = f4;
            f2.NeighbourDown = f6;
            f3.NeighbourDown = f2;
            f4.NeighbourDown = f1;
            f5.NeighbourDown = f3;
            f6.NeighbourDown = f5;

            f1.NeighbourLeft = f6;
            f2.NeighbourLeft = f5;
            f3.NeighbourLeft = f5;
            f4.NeighbourLeft = f3;
            f5.NeighbourLeft = f2;
            f6.NeighbourLeft = f2;

            f1.NeighbourRight = f3;
            f2.NeighbourRight = f1;
            f3.NeighbourRight = f1;
            f4.NeighbourRight = f6;
            f5.NeighbourRight = f4;
            f6.NeighbourRight = f4;
        }

        public List<T> GetNeighboursUp(T node)
        {
            return null;
        }

        public List<T> GetNeighboursRight(T node)
        {
            return null;
        }

        public List<T> GetNeighboursDown(T node)
        {
            return null;
        }

        public List<T> GetNeighboursLeft(T node)
        {
            return null;
        }

        /// <summary>
        /// Splits a node in four new nodes
        /// </summary>
        /// <param name="node">The node to split</param>
        public void Split(T node)
        {

        }

        public IEnumerable<T> GetArea(T child, int depth)
        {
            CircularQuadNode node = nodes[child];
            if (node.Depth < depth)
                throw new ArgumentOutOfRangeException("Depth is lower than child depth");

            while (node.Depth > depth)
                node = node.Parent;

            return GetArea(node, depth);
        }

        protected IEnumerable<T> GetArea(CircularQuadNode node, int depth)
        {
            if(node == null)
                return new T[]{};

            return new T[] { node.Value }
                .Concat(GetArea(node.NeighbourUp, depth + 1))
                .Concat(GetArea(node.NeighbourRight, depth + 1))
                .Concat(GetArea(node.NeighbourDown, depth + 1))
                .Concat(GetArea(node.NeighbourLeft, depth + 1));
        }
        
        /// <summary>
        /// Merges the parents of <paramref name="child"/> until depth <paramref name="untilDepth"/> and replaces it with <paramref name="replacement"/>
        /// </summary>
        /// <param name="child">The child node to find</param>
        /// <param name="untilDepth">The depth to merge to</param>
        /// <param name="replacement">The node to replace it with</param>
        /// <returns>The list of merged nodes</returns>
        public List<T> Merge(T child, int untilDepth, T replacement)
        {
            return null;
        }

        /// <summary>
        /// Replaces <paramref name="node"/> with <paramref name="replacement"/>
        /// </summary>
        /// <param name="node">The node to find</param>
        /// <param name="replacement">The node that replaces the found node</param>
        public void Replace(T node, int depth, T replacement)
        {

        }

        /// <summary>
        /// Returns how deep the node is placed (size)
        /// </summary>
        /// <param name="node">The node to look up</param>
        /// <returns>Zero based depth</returns>
        public int GetDepth(T node)
        {
            return 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return nodes.Keys.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return nodes.Keys.GetEnumerator();
        }

        protected class CircularQuadNode
        {
            public CircularQuadNode Parent { get; set; }

            public int Depth { get; set; }

            public CircularQuadNode NeighbourUp { get; set; }
            public CircularQuadNode NeighbourRight { get; set; }
            public CircularQuadNode NeighbourDown { get; set; }
            public CircularQuadNode NeighbourLeft { get; set; }

            public T Value { get; set; }

            public CircularQuadNode ChildTopLeft { get; set; }
            public CircularQuadNode ChildTopRight { get; set; }
            public CircularQuadNode ChildBottomRight { get; set; }
            public CircularQuadNode ChildBottomLeft { get; set; }

            public bool IsLeaf
            {
                get
                {
                    return ChildTopLeft == null && ChildTopRight == null && ChildBottomRight == null && ChildBottomLeft == null;
                }
            }
        }
    }
}
