using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jaws.Data
{
    public class WeatherLayer : ICloneable
    {
        /// <summary>
        /// Temperature of this layer in Kelvin
        /// </summary>
        public Single Temperature { get; set; }

        /// <summary>
        /// Humidity of this layer in Saturation percentage
        /// </summary>
        public Single Humidity { get; set; }

        /// <summary>
        /// Air pressure of this layer in hectoPascal( hPa/millibar )
        /// </summary>
        public Single Pressure { get; set; }

        /// <summary>
        /// Precipitation type
        /// </summary>
        public PrecipitationType Precipitation { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        protected WeatherLayer()
        {

        }

        /// <summary>
        /// Generates a weather layer
        /// </summary>
        /// <param name="temperature">Temperature in Kelvin</param>
        /// <param name="humidity">Humidity in percentage</param>
        /// <param name="pressure">Air pressure in hectoPascal</param>
        /// <param name="precipitation"></param>
        /// <returns>The weather layer</returns>
        public static WeatherLayer Generate( Single temperature, Single humidity, Single pressure, PrecipitationType precipitation)
        {
            return new WeatherLayer()
            {
                Temperature = temperature,
                Humidity = humidity,
                Precipitation = precipitation,
                Pressure = pressure
            };
        }

        /// <summary>
        /// Returns a deep clone of this layer
        /// </summary>
        /// <returns></returns>
        public WeatherLayer DeepClone()
        {
            return WeatherLayer.Generate(this.Temperature, this.Humidity, this.Pressure, this.Precipitation);
        }

        /// <summary>
        /// Clones the object
        /// </summary>
        /// <returns></returns>
        Object ICloneable.Clone()
        {
            return DeepClone();
        }

        /// <summary>
        /// Checks if this layer has equals values as object
        /// </summary>
        /// <param name="obj">Another layer</param>
        /// <returns>true when equals</returns>
        public override bool Equals(object obj)
        {
            var that = obj as WeatherLayer;
            if (that == null)
                return false;

            return that.Temperature == this.Temperature &&
                that.Humidity == this.Humidity &&
                that.Precipitation == this.Precipitation &&
                that.Pressure == this.Pressure;
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (this.Temperature.GetHashCode() * 127) ^
                (this.Humidity.GetHashCode() * 251) ^
                (this.Pressure.GetHashCode() * 83) ^
                (this.Precipitation.GetHashCode() * 17);
        }
    }
}
