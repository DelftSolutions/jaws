using System;
using System.Collections.Generic;
using System.Collections;
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
        /// Creates a flat circularquadtree (2 faces instead of 6)
        /// </summary>
        /// <param name="f1"></param>
        public CircularQuadTree(T face1)
        {
            nodes = new Dictionary<T, CircularQuadNode>();

            var f1 = new CircularQuadNode();
            f1.Value = face1;

            f1.NeighbourUp = f1;
            f1.NeighbourRight = f1;
            f1.NeighbourDown = f1;
            f1.NeighbourLeft = f1;

            nodes.Add(face1, f1);
        }

        protected IEnumerable<CircularQuadNode> GetSide(CircularQuadNode node, int x, int y)
        {
            if (node == null)
                yield break;

            var directions = GetSideDirections(x, y);
            Queue<CircularQuadNode> q = new Queue<CircularQuadNode>();
            q.Enqueue(node);

            while (q.Count > 0)
            {
                var n = q.Dequeue();

                if (n == null)
                    continue;

                foreach (var direction in directions)
                    q.Enqueue(n[direction[0], direction[1]]);

                yield return n;
            }
        }

        protected int[][] GetSideDirections(int x, int y)
        {
            if (x == 0)
                return new int[][] { new int[] { -1, y }, new int[] { 1, y } };
            if (y == 0)
                return new int[][] { new int[] { x, -1 }, new int[] { x, 1 } };

            throw new ArgumentOutOfRangeException();
        }

        public IEnumerable<T> GetNeighbours(T node, int x, int y)
        {
            var parent = nodes[node];

            return GetNeighbourNodes(parent, x, y).Select((n) => n.Value).Where((n) => n != null);
        }

        protected IEnumerable<CircularQuadNode> GetNeighbourNodes(CircularQuadNode node, int x, int y)
        {
            var neighbour = node[x, y];
            return GetSide(neighbour, -x, -y);
        }

        /// <summary>
        /// Splits a node in four new nodes
        /// </summary>
        /// <param name="node">The node to split</param>
        public IQuadNode[] Split(T node)
        {
            var parent = nodes[node];
            int newdepth = parent.Depth + 1;
            
            //Remove old value
            parent.Value = default(T);
            nodes.Remove(node);

            var newnodes = node.Split();

            var temp = new CircularQuadNode[2, 2];

            //Create new nodes
            for(int i = 0; i <= 1; i++)
                for (int j = 0; j <= 1; j++)
                {
                    int x = i * 2 - 1;
                    int y = j * 2 - 1;

                    T val = (T)newnodes[j * 2 + i];
                    var n = new CircularQuadNode()
                    {
                        Depth = parent.Depth + 1,
                        Parent = parent,
                        Value = val,
                    };
                    parent[x, y] = n;
                    nodes.Add(val, n);
                    temp[i, j] = n;

                    var masks = new int[][] { new int[] { 1, 0 }, new int[] { 0, 1 } };
                    foreach (var mask in masks)
                    {
                        int neighx = x * mask[0];
                        int neighy = y * mask[1];

                        //Get parent neighbour
                        var neighbour = parent[neighx, neighy];
                        //Get correct child
                        neighbour = neighbour[x - 2 * neighx, y - 2 * neighy];
                        n[neighx, neighy] = neighbour ?? parent[neighx, neighy];
                        GetSide(neighbour, -neighx, -neighy).AsParallel().ForAll((s) => s[-neighx, -neighy] = n);
                    }
                }

            // Set neighbours
            for(int i = 0; i <= 1; i++)
                for (int j = 0; j <= 1; j++)
                {
                    int x = i * 2 - 1;
                    int y = j * 2 - 1;
                    var n = temp[i, j];
                    n[-x, 0] = temp[1 - i, j];
                    n[0, -y] = temp[i, 1 - j];
                }

            return newnodes;
        }

        public IEnumerable<T> GetArea(T child, int depth)
        {
            return GetArea(GetParent(child, depth));
        }

        protected CircularQuadNode GetParent(T child, int depth)
        {
            CircularQuadNode node = nodes[child];
            if (node.Depth < depth)
                throw new ArgumentOutOfRangeException("Depth is lower than child depth");

            while (node.Depth > depth)
                node = node.Parent;

            return node;
        }

        protected IEnumerable<T> GetArea(CircularQuadNode node)
        {
            return GetChildren(node).Select((n) => n.Value).Where((n) => n != null);
        }

        protected IEnumerable<CircularQuadNode> GetChildren(CircularQuadNode node)
        {
            if (node == null)
                yield break;

            Queue<CircularQuadNode> q = new Queue<CircularQuadNode>();
            q.Enqueue(node);

            while (q.Count > 0)
            {
                var n = q.Dequeue();
                if (n == null)
                    continue;

                for (int i = 0; i <= 1; i++)
                    for (int j = 0; j <= 1; j++)
                    {
                        int x = i * 2 - 1;
                        int y = j * 2 - 1;
                        q.Enqueue(n[x, y]);
                    }

                yield return n;
            }
        }

        /// <summary>
        /// Replaces <paramref name="node"/> with <paramref name="replacement"/>
        /// </summary>
        /// <param name="node">The node to find</param>
        /// <param name="replacement">The node that replaces the found node</param>
        public void Replace(T node, int depth, T replacement)
        {
            var parent = GetParent(node, depth);

            var directions = new int[][] { new int[] { 1, 0 }, new int[] { 0, -1 }, new int[] { -1, 0 }, new int[] { 0, 1 } };

            //Skip first node (parent)
            var deleted = GetChildren(parent).Skip(1);

            //Remove all references to soon to be deleted nodes
            foreach (var direction in directions)
            {
                int x = direction[0];
                int y = direction[1];
                var neighbour = parent[x, y];

                if (neighbour.Depth > parent.Depth)
                    continue;

                var sideNodes = GetNeighbourNodes(parent, x, y);
                foreach (var edge in sideNodes)
                    edge[-x, -y] = parent;
            }

            //Remove children
            parent.ChildBottomLeft = null;
            parent.ChildBottomRight = null;
            parent.ChildTopLeft = null;
            parent.ChildTopRight = null;

            foreach (var child in deleted)
                nodes.Remove(child.Value);

            //Add replacement
            parent.Value = replacement;
            nodes.Add(replacement, parent);
        }

        /// <summary>
        /// Returns how deep the node is placed (size)
        /// </summary>
        /// <param name="node">The node to look up</param>
        /// <returns>Zero based depth</returns>
        public int GetDepth(T node)
        {
            return nodes[node].Depth;
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
            public CircularQuadNode()
            {
                Depth = 0;
                References = new CircularQuadNode[3, 3];
            }

            public CircularQuadNode this[int x, int y]
            {
                get { return References[x + 1, y + 1]; }
                set { References[x + 1, y + 1] = value; }
            }

            public CircularQuadNode[,] References { get; protected set; }

            public int Depth { get; set; }
            public T Value { get; set; }

            public CircularQuadNode Parent
            {
                get { return this[0, 0]; }
                set { this[0, 0] = value; }
            }

            public CircularQuadNode ChildTopLeft { set { this[-1, 1] = value; }}
            public CircularQuadNode ChildTopRight { set { this[1, 1] = value; } }
            public CircularQuadNode ChildBottomLeft { set { this[-1, -1] = value; } }
            public CircularQuadNode ChildBottomRight { set { this[1, -1] = value; } }

            public CircularQuadNode NeighbourUp { set { this[0, 1] = value; } }
            public CircularQuadNode NeighbourRight { set { this[1, 0] = value; } }
            public CircularQuadNode NeighbourDown { set { this[0, -1] = value; } }
            public CircularQuadNode NeighbourLeft { set { this[-1, 0] = value; } }

            public override string ToString()
            {
                return "CircularQuadNode(){ " + Value.ToString() + " }";
            }

            public bool IsLeaf
            {
                get
                {
                    for (int i = 0; i <= 1; i++)
                        for (int j = 0; j <= 1; j++)
                        {
                            int x = i * 2 - 1;
                            int y = j * 2 - 1;
                            if (this[x, y] != null)
                                return false;
                        }
                    return true;
                }
            }
        }
    }
}
