using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jaws.Generators;

namespace Test
{
    /// <summary>
    /// Summary description for WorldGeneratorTest
    /// </summary>
    [TestClass]
    public class WorldGeneratorTest
    {
        [TestMethod]
        public void TestGenerate()
        {
            var world = WorldGenerator.Generate();
        }
    }
}
