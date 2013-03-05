using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jaws.Data;

namespace Test
{
    [TestClass]
    public class WeatherLayerTest
    {
        /// <summary>
        /// Tests generation of layers
        /// </summary>
        [TestMethod]
        public void TestGenerate()
        {
            var layer = WeatherLayer.Generate(12, 55, 1305, PrecipitationType.Rain);
            Assert.IsInstanceOfType(layer, typeof(WeatherLayer), "WeatherLayer was not generated");
        }

        /// <summary>
        /// Tests layer temperature
        /// </summary>
        [TestMethod]
        public void TestTemperature()
        {
            var layerA = WeatherLayer.Generate(0, 0, 0, PrecipitationType.None);
            var layerB = WeatherLayer.Generate(2.5f, 0, 0, PrecipitationType.None);

            Assert.AreEqual(0, layerA.Temperature, "temperature was not correctly set");
            Assert.AreEqual(2.5f, layerB.Temperature, "temperature was not correctly set");
        }

        /// <summary>
        /// Tests layer humidity
        /// </summary>
        [TestMethod]
        public void TestHumidity()
        {
            var layerA = WeatherLayer.Generate(0, 0, 0, PrecipitationType.None);
            var layerB = WeatherLayer.Generate(0, 2.5f, 0, PrecipitationType.None);

            Assert.AreEqual(0, layerA.Humidity, "humidity was not correctly set");
            Assert.AreEqual(2.5f, layerB.Humidity, "humidity was not correctly set");
        }

        /// <summary>
        /// Tests layer pressure
        /// </summary>
        [TestMethod]
        public void TestPressure()
        {
            var layerA = WeatherLayer.Generate(0, 0, 0, PrecipitationType.None);
            var layerB = WeatherLayer.Generate(0, 0, 2.5f, PrecipitationType.None);

            Assert.AreEqual(0, layerA.Pressure, "pressure was not correctly set");
            Assert.AreEqual(2.5f, layerB.Pressure, "pressure was not correctly set");
        }

        /// <summary>
        /// Tests layer precipitation
        /// </summary>
        [TestMethod]
        public void TestPrecipitation()
        {
            var layerA = WeatherLayer.Generate(0, 0, 0, PrecipitationType.None);
            var layerB = WeatherLayer.Generate(0, 0, 0, PrecipitationType.Rain);

            Assert.AreEqual(PrecipitationType.None, layerA.Precipitation, "precipitation was not correctly set");
            Assert.AreEqual(PrecipitationType.Rain, layerB.Precipitation, "precipitation was not correctly set");
        }

        /// <summary>
        /// Tests deepcloning the layer
        /// </summary>
        [TestMethod]
        public void TestDeepClone()
        {
            var parent = WeatherLayer.Generate(12, 55, 1305, PrecipitationType.Rain);
            var clone = parent.DeepClone();

            Assert.AreNotSame(parent, clone);

            // Test all the properties
            Assert.AreEqual(parent.Temperature, clone.Temperature, "temperature was not correctly cloned");
            Assert.AreEqual(parent.Humidity, clone.Humidity, "humidity was not correctly cloned");
            Assert.AreEqual(parent.Precipitation, clone.Precipitation, "precipitation was not correctly cloned");
            Assert.AreEqual(parent.Pressure, clone.Pressure, "pressure was not correctly cloned");
        }

        /// <summary>
        /// Tests cloning the layer
        /// </summary>
        [TestMethod]
        public void TestClone()
        {
            var layer = WeatherLayer.Generate(12, 55, 1305, PrecipitationType.Rain);
            Assert.AreEqual(layer.DeepClone(), ((ICloneable)layer).Clone());
        }

         /// <summary>
        /// Tests cloning the layer
        /// </summary>
        [TestMethod]
        public void TestEquals()
        {
            var layerA = WeatherLayer.Generate(12, 55, 1305, PrecipitationType.Rain);
            var layerB = WeatherLayer.Generate(12, 55, 1305, PrecipitationType.Rain);
            var layerC = WeatherLayer.Generate(0, 0, 0, PrecipitationType.None);
            var layerD = WeatherLayer.Generate(0, 55, 1305, PrecipitationType.Rain);
            var layerE = WeatherLayer.Generate(12, 0, 1305, PrecipitationType.Rain);
            var layerF = WeatherLayer.Generate(12, 55, 0, PrecipitationType.Rain);
            var layerG = WeatherLayer.Generate(12, 55, 1305, PrecipitationType.None);
            var cloneA = layerA.DeepClone();

            Assert.IsFalse(layerA.Equals(null));
            Assert.IsFalse(layerA.Equals("nothing"));
            Assert.IsTrue(layerA.Equals(layerB));
            Assert.IsTrue(layerB.Equals(layerA));
            Assert.IsFalse(layerA.Equals(layerC));
            Assert.IsFalse(layerC.Equals(layerA));
            Assert.IsFalse(layerA.Equals(layerD));
            Assert.IsFalse(layerD.Equals(layerA));
            Assert.IsFalse(layerA.Equals(layerE));
            Assert.IsFalse(layerE.Equals(layerA));
            Assert.IsFalse(layerA.Equals(layerF));
            Assert.IsFalse(layerF.Equals(layerA));
            Assert.IsFalse(layerA.Equals(layerG));
            Assert.IsFalse(layerG.Equals(layerA));
            Assert.IsTrue(layerA.Equals(cloneA));
            Assert.IsTrue(cloneA.Equals(layerA));
        }

        /// <summary>
        /// Tests hashcodes
        /// </summary>
        [TestMethod]
        public void TestGetHashCode()
        {
            var layerA = WeatherLayer.Generate(12, 55, 1305, PrecipitationType.Rain);
            var layerB = WeatherLayer.Generate(12, 55, 1305, PrecipitationType.Rain);
            var layerC = WeatherLayer.Generate(0, 0, 0, PrecipitationType.None);
            var layerD = layerA;
            var cloneA = layerA.DeepClone();

            Assert.AreEqual(layerA.GetHashCode(), layerB.GetHashCode());
            Assert.AreNotEqual(layerA.GetHashCode(), layerC.GetHashCode());
            Assert.AreEqual(layerA.GetHashCode(), layerD.GetHashCode());
            Assert.AreEqual(layerA.GetHashCode(), cloneA.GetHashCode());
        }

    }
}
