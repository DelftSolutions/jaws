﻿using System;
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

        protected IEnumerable<CircularQuadNode> GetSide(CircularQuadNode node, int x, int y)
        {
            if (node == null)
                return new CircularQuadNode[] { };

            int x1 = (x + 1) / 2;
            int y1 = (y + 1) / 2;

            int x2 = Math.Abs(x1 + y);
            int y2 = Math.Abs(y1 + x);

            return new CircularQuadNode[] { node }
                .Concat(GetSide(node.Children[x1, y1], x, y))
                .Concat(GetSide(node.Children[x2, y2], x, y));
        }


        public IEnumerable<T> GetNeighbours(T node, int x, int y)
        {
            var parent = nodes[node];
            return GetSide(parent, x, y).Select((n) => n.Value).Where((n) => n != null);
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
                    parent.Children[i, j] = n;
                    nodes.Add(val, n);

                    

                    //Fix references to me
                    
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

            var res = new CircularQuadNode[] { node };
            for(int x = 0; x <= 1; x++)
                for(int y = 0; y <= 1; y++)
                    res.Concat(GetChildren(node.Children[x,y]));

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

            //Skip first node (parent)
            var deleted = GetChildren(parent).Skip(1);

            //Remove all references to soon to be deleted nodes
            //var leftNeighbours = GetRightNodes(parent.NeighbourLeft).Skip(1);
            //leftNeighbours.AsParallel().ForAll((n) => ReplaceRightReference(n, deleted, parent));

            //var rightNeighbours = GetLeftNodes(parent.NeighbourRight).Skip(1);
            //rightNeighbours.AsParallel().ForAll((n) => ReplaceLeftReference(n, deleted, parent));

            //var topNeighbours = GetBottomNodes(parent.NeighbourUp).Skip(1);
            //topNeighbours.AsParallel().ForAll((n) => ReplaceDownReference(n, deleted, parent));

            //var bottomNeighbours = GetTopNodes(parent.NeighbourDown).Skip(1);
            //bottomNeighbours.AsParallel().ForAll((n) => ReplaceUpReference(n, deleted, parent));

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
                Neighbours = new CircularQuadNode[2, 2];
                Children = new CircularQuadNode[2, 2];
            }

            public CircularQuadNode Parent { get; set; }
            public int Depth { get; set; }
            public CircularQuadNode[,] Neighbours { get; protected set; }
            public T Value { get; set; }
            public CircularQuadNode[,] Children { get; protected set; }

            public CircularQuadNode ChildTopLeft { set { Children[0, 0] = value; } }
            public CircularQuadNode ChildTopRight { set { Children[1, 0] = value; } }
            public CircularQuadNode ChildBottomLeft { set { Children[0, 1] = value; } }
            public CircularQuadNode ChildBottomRight { set { Children[1, 1] = value; } }

            public CircularQuadNode NeighbourUp { set { Neighbours[0, 0] = value; } }
            public CircularQuadNode NeighbourRight { set { Neighbours[1, 0] = value; } }
            public CircularQuadNode NeighbourDown { set { Neighbours[0, 1] = value; } }
            public CircularQuadNode NeighbourLeft { set { Neighbours[1, 1] = value; } }

            public bool IsLeaf
            {
                get
                {
                    for (int x = 0; x <= 1; x++)
                        for (int y = 0; y <= 1; y++)
                            if (Children[x, y] != null)
                                return false;
                    return true;
                }
            }
        }
    }
}
