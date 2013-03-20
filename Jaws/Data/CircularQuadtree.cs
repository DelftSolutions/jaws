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
            nodes = new Dictionary<T, CircularQuadNode>();

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

            nodes.Add(f1.Value, f1);
            nodes.Add(f2.Value, f2);
            nodes.Add(f3.Value, f3);
            nodes.Add(f4.Value, f4);
            nodes.Add(f5.Value, f5);
            nodes.Add(f6.Value, f6);
        }

        /// <summary>
        /// Creates a flat circularquadtree (2 faces instead of 6)
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        public CircularQuadTree(T face1, T face2)
        {
            nodes = new Dictionary<T, CircularQuadNode>();

            var f1 = new CircularQuadNode();
            f1.Value = face1;
            var f2 = new CircularQuadNode();
            f2.Value = face2;

            f1.NeighbourUp = f2;
            f1.NeighbourRight = f2;
            f1.NeighbourDown = f2;
            f1.NeighbourLeft = f2;

            f2.NeighbourUp = f1;
            f2.NeighbourRight = f1;
            f2.NeighbourDown = f1;
            f2.NeighbourLeft = f1;

            nodes.Add(face1, f1);
            nodes.Add(face2, f2);
        }

        protected IEnumerable<CircularQuadNode> GetSide(CircularQuadNode node, int x, int y)
        {
            if (node == null)
                return new CircularQuadNode[] { };

            var directions = GetSideDirections(x, y);

            return new CircularQuadNode[] { node }
                .Concat(GetSide(node[directions[0][0], directions[0][1]], x, y))
                .Concat(GetSide(node[directions[1][0], directions[1][1]], x, y));
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
            IEnumerable<CircularQuadNode> result = new CircularQuadNode[] { };
            IEnumerable<CircularQuadNode> parents = new CircularQuadNode[] { };
            CircularQuadNode cur = neighbour;
            while (cur != null)
            {
                parents = parents.Concat(new CircularQuadNode[] { cur });
                cur = cur.Parent;
            }

            var directions = new int[][] { new int[] { 1, 0 }, new int[] { 0, -1 }, new int[] { -1, 0 }, new int[] { 0, 1 } };
            foreach(var direction in directions)
            {
                if(parents.Contains(neighbour[direction[0], direction[1]]))
                    result = result.Concat(GetSide(neighbour, direction[0], direction[1]));
            }
            return result;
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
                return new CircularQuadNode[] { };

            IEnumerable<CircularQuadNode> res = new CircularQuadNode[] { node };
            for(int i = 0; i <= 1; i++)
                for (int j = 0; j <= 1; j++)
                {
                    int x = i * 2 - 1;
                    int y = j * 2 - 1;
                    res = res.Concat(GetChildren(node[x, y]));
                }

            return res;
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
                var sideNodes = GetNeighbourNodes(parent, direction[0], direction[1]);
                //var neighbour = parent[direction[0], direction[1]];
                //int x = -direction[0];
                //int y = -direction[1];
                //var sideNodes = GetSide(neighbour, x, y);
                sideNodes.AsParallel().ForAll((n) =>
                {
                    foreach (var neighbourdirection in directions)
                    {
                        foreach(var d in deleted)
                            if (n[neighbourdirection[0], neighbourdirection[1]] == d)
                            {
                                n[neighbourdirection[0], neighbourdirection[1]] = parent;
                                break;
                            }
                    }
                });
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
