using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jaws.Data
{
    public interface IQuadNode
    {
        Tuple<IQuadNode, IQuadNode, IQuadNode, IQuadNode> Split();
    }
}
