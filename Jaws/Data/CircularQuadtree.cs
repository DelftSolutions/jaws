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
    public class CircularQuadtree<T> : IEnumerable<T> where T: IQuadNode
    {
        protected Dictionary<T, CircularQuadNode> nodes;

        /// <summary>
        /// Creates a new CircularQuadTree
        /// </summary>
        public CircularQuadtree(T face1, T face2, T face3, T face4, T face5, T face6)
        {
        }

        /// <summary>
        /// Returns the neighbours of a given node
        /// </summary>
        /// <param name="node">The node to look up</param>
        /// <returns>The neighbours of the node</returns>
        public Tuple<T, T, T, T> GetNeighbours(T node)
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
        public void Replace(T node, T replacement)
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

            public bool IsLeaf()
            {
                return ChildTopLeft == null && ChildTopRight == null && ChildBottomRight == null && ChildBottomLeft == null;
            }
        }
    }
}
