using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jaws.Data;

namespace Test
{
    [TestClass]
    public class WeatherNodeTest
    {
        /// <summary>
        /// Tests generation of nodes
        /// </summary>
        [TestMethod]
        public void TestGenerate()
        {
            var node = WeatherNode.Generate(1);
            Assert.IsInstanceOfType(node, typeof(WeatherNode), "weathernode was not generated");
        }

        /// <summary>
        /// Tests the ground temperature
        /// </summary>
        [TestMethod]
        public void TestTemperature()
        {
            var nodeA = WeatherNode.Generate(1, 0, 0, 0, PrecipitationType.None);
            var nodeB = WeatherNode.Generate(1, 2.5f, 0, 0, PrecipitationType.None);

            Assert.AreEqual(0, nodeA.Temperature, "temperature was not correctly set");
            Assert.AreEqual(2.5f, nodeB.Temperature, "temperature was not correctly set");
        }

        /// <summary>
        /// Tests the ground humidity
        /// </summary>
        [TestMethod]
        public void TestHumidity()
        {
            var nodeA = WeatherNode.Generate(1, 0, 0, 0, PrecipitationType.None);
            var nodeB = WeatherNode.Generate(1, 0, 2.5f, 0, PrecipitationType.None);

            Assert.AreEqual(0, nodeA.Humidity, "humidity was not correctly set");
            Assert.AreEqual(2.5f, nodeB.Humidity, "humidity was not correctly set");
        }

        /// <summary>
        /// Tests the ground pressure
        /// </summary>
        [TestMethod]
        public void TestPressure()
        {
            var nodeA = WeatherNode.Generate(1, 0, 0, 0, PrecipitationType.None);
            var nodeB = WeatherNode.Generate(1, 0, 0, 2.5f, PrecipitationType.None);

            Assert.AreEqual(0, nodeA.Pressure, "pressure was not correctly set");
            Assert.AreEqual(2.5f, nodeB.Pressure, "pressure was not correctly set");
        }

        /// <summary>
        /// Tests the ground precipitation
        /// </summary>
        [TestMethod]
        public void TestPrecipitation()
        {
            var nodeA = WeatherNode.Generate(1, 0, 0, 0, PrecipitationType.None);
            var nodeB = WeatherNode.Generate(1, 0, 0, 0, PrecipitationType.Rain);

            Assert.AreEqual(PrecipitationType.None, nodeA.Precipitation, "precipitation was not correctly set");
            Assert.AreEqual(PrecipitationType.Rain, nodeB.Precipitation, "precipitation was not correctly set");
        }

        /// <summary>
        /// Tests splitting the node
        /// </summary>
        [TestMethod]
        public void TestSplit()
        {
            var parent = WeatherNode.Generate(1, 12, 55, 1305, PrecipitationType.None);
            var splitted = parent.Split();

            // Actually split
            Assert.AreNotSame(parent, splitted[0], "splitted object has the same reference as parent");
            Assert.AreNotSame(parent, splitted[1], "splitted object has the same reference as parent");
            Assert.AreNotSame(parent, splitted[2], "splitted object has the same reference as parent");
            Assert.AreNotSame(parent, splitted[3], "splitted object has the same reference as parent");

            // Cloned objects
            Assert.AreNotSame(splitted[1], splitted[0], "splitted object has same reference as other splitted object");
            Assert.AreNotSame(splitted[2], splitted[0], "splitted object has same reference as other splitted object");
            Assert.AreNotSame(splitted[3], splitted[0], "splitted object has same reference as other splitted object");
            Assert.AreNotSame(splitted[2], splitted[1], "splitted object has same reference as other splitted object");
            Assert.AreNotSame(splitted[3], splitted[1], "splitted object has same reference as other splitted object");
            Assert.AreNotSame(splitted[3], splitted[2], "splitted object has same reference as other splitted object");

            // Cloned values?
            var topleft = splitted[0] as WeatherNode;
            var topright = splitted[1] as WeatherNode;
            var bottomright = splitted[2] as WeatherNode;
            var bottomleft = splitted[3] as WeatherNode;

            Assert.AreEqual(topleft.Area, parent.Area / 4, "area is not splitted in 4 equal areas");
            Assert.AreEqual(topright.Area, parent.Area / 4, "area is not splitted in 4 equal areas");
            Assert.AreEqual(bottomright.Area, parent.Area / 4, "area is not splitted in 4 equal areas");
            Assert.AreEqual(bottomleft.Area, parent.Area / 4, "area is not splitted in 4 equal areas");

            // Rest should be tested in Unittest of WeatherLayer
        }

        /// <summary>
        /// Tests deepcloning the node
        /// </summary>
        [TestMethod]
        public void TestDeepClone()
        {
            var parent = WeatherNode.Generate(1, 12, 55, 1305, PrecipitationType.Rain);
            var clone = parent.DeepClone();

            Assert.AreEqual( parent.Area, clone.Area, "area doesn't match");

            // Test all the layers
            var clone_enumerator = clone.GetEnumerator();
            var has_clone = clone_enumerator.MoveNext();
            foreach(var layer in parent) {
                Assert.IsTrue(has_clone, "no clone layer, but there is a parent layer");
                Assert.AreNotSame(layer, clone_enumerator.Current);
                Assert.AreEqual(layer, clone_enumerator.Current);
                has_clone = clone_enumerator.MoveNext();
            }

            Assert.IsFalse(has_clone, "clone layer, but there is no parent layer");
        }

        /// <summary>
        /// Tests cloning the node
        /// </summary>
        [TestMethod]
        public void TestClone()
        {
            var node = WeatherNode.Generate(1, 12, 55, 1305, PrecipitationType.Rain);
            Assert.AreEqual(node.DeepClone(), ((ICloneable)node).Clone());
        }

        /// <summary>
        /// Tests Enumerable methods
        /// </summary>
        [TestMethod]
        public void TestGetEnumerator()
        {
            var node = WeatherNode.Generate(1, 12, 55, 1305, PrecipitationType.Rain);

            Assert.IsInstanceOfType(node.GetEnumerator(), typeof(System.Collections.Generic.IEnumerator<WeatherLayer>));
            Assert.AreEqual(node.GetEnumerator(), ((System.Collections.IEnumerable)node).GetEnumerator());
        }

        /// <summary>
        /// Equality tests
        /// </summary>
        [TestMethod]
        public void TestEquals()
        {
            var nodeA = WeatherNode.Generate(1, 12, 55, 1305, PrecipitationType.Rain);
            var nodeB = WeatherNode.Generate(1, 12, 55, 1305, PrecipitationType.Rain);
            var nodeC = WeatherNode.Generate(1, 0, 0, 0, PrecipitationType.None);
            var cloneA = nodeA.DeepClone();

            Assert.IsFalse(nodeA.Equals(null));
            Assert.IsFalse(nodeA.Equals("nothing"));
            Assert.IsTrue(nodeA.Equals(nodeB));
            Assert.IsTrue(nodeB.Equals(nodeA));
            Assert.IsFalse(nodeA.Equals(nodeC));
            Assert.IsFalse(nodeC.Equals(nodeA));
            Assert.IsTrue(nodeA.Equals(cloneA));
            Assert.IsTrue(cloneA.Equals(nodeA));

            var nodeD = nodeA.DeepClone();
            var nodeE = nodeA.DeepClone();
            var nodeF = nodeA.DeepClone();
            nodeD.Layers[0].Temperature = 1;
            nodeE.Layers.Add(WeatherLayer.Generate(WeatherNode.GroundLayerHeight, 1, 0, 0, PrecipitationType.None));
            nodeF.Layers.RemoveAt(0);

            Assert.IsFalse(nodeA.Equals(nodeD));
            Assert.IsFalse(nodeD.Equals(nodeA));
            Assert.IsFalse(nodeA.Equals(nodeF));
            Assert.IsFalse(nodeE.Equals(nodeA));
            Assert.IsFalse(nodeA.Equals(nodeF));
            Assert.IsFalse(nodeF.Equals(nodeA));
        }

        /// <summary>
        /// Tests hashcodes
        /// </summary>
        [TestMethod]
        public void TestGetHashCode()
        {
            var nodeA = WeatherNode.Generate(1, 12, 55, 1035, PrecipitationType.Rain);
            var nodeB = WeatherNode.Generate(1, 12, 55, 1035, PrecipitationType.Rain);
            var nodeC = WeatherNode.Generate(1, 0, 0, 0, PrecipitationType.None);
            var nodeD = nodeA;
            var cloneA = nodeA.DeepClone();

            Assert.AreEqual(nodeA.GetHashCode(), nodeB.GetHashCode());
            Assert.AreNotEqual(nodeA.GetHashCode(), nodeC.GetHashCode());
            Assert.AreEqual(nodeA.GetHashCode(), nodeD.GetHashCode());
            Assert.AreEqual(nodeA.GetHashCode(), cloneA.GetHashCode());
        }
    }
}
