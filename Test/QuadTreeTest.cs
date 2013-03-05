using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jaws.Data;

namespace Test
{
    [TestClass]
    public class QuadTreeTest
    {
        [TestMethod]
        public void InitTest()
        {
            
        }

        class TestNode : IQuadNode
        {

            Tuple<IQuadNode, IQuadNode, IQuadNode, IQuadNode> IQuadNode.Split()
            {
                throw new NotImplementedException();
            }
        }
    }
}
