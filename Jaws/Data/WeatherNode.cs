using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jaws.Data
{
    /// <summary>
    /// 
    /*
        * Cooling air to dew point
        * 1. Adiabatic cooling: Rising (and thus expanding) air, due to convection, large-scale atmospheric motions or orographic lift (say a mountains).
        * 2. Conductive cooling: Air comes in contact with colder surface, for example from liquid water surface to colder land.
        * 3. Radiational cooling: Emission of infrared radition either by the air or the surface underneath.
        * 4. Evaporative cooling: When moisture is added to the air through evaporation, which forces the temperature to cool, until it reaches saturation or wet-bulb temperature.
        * 
        * Adding moisture/Increasing saturation
        * 1. Wind convergence into areas of upward motion
        * 2. Precipitation or virga falling from above
        * 3. Daytime heating which evaporates water from water bodies
        * 4. Transpiration from vegitation
        * 5. Cool or Dry air moving over warmer water
        * 6. Lifting air over mountains
        * 
        * Formation of
        * 1. Raindrops: coalescense of water droplets to larger water droplets, when air turbulence occurs.
        * 2. Ice pellets: layer of above feeezing air exists with subfreezing air both above and below it
        * 3. Hail: Supercooled cloud droplets freeze on contact with dust or dirt.
        * 4. Snowflakes: Supercooled cloud droplets freeze and grow in supersaturated area with water.
        * 
    */
    /// </summary>
    public class WeatherNode : IQuadNode, ICloneable
    {
        /// <summary>
        /// The current temperature in Kelvin
        /// </summary>
        public Single Temperature { get; set; }

        /// <summary>
        /// The current Humidity in Saturation percentage
        /// </summary>
        public Single Humidity { get; set; }

        /// <summary>
        /// The current Airpressure in hectoPascal (hPa/millibar)
        /// </summary>
        public Single Pressure { get; set; }

        /// <summary>
        /// The current precipitation (Weather by Water Vapour)
        /// </summary>
        public ICloneable Precipitation { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        protected WeatherNode() { }

        /// <summary>
        /// Generates a weathernode
        /// </summary>
        /// <param name="temperature">Temperature in Kelvin</param>
        /// <param name="humidity">Humidity in percentage</param>
        /// <param name="pressure">Air Pressure in hectoPascal</param>
        /// <returns>The generated weathernode</returns>
        public static WeatherNode Generate(Single temperature, Single humidity, Single pressure)
        {
            return new WeatherNode() { Temperature = temperature, Humidity = humidity, Pressure = pressure };
        }

        /// <summary>
        /// Splits this node in four smaller nodes
        /// </summary>
        /// <returns></returns>
        public Tuple<IQuadNode, IQuadNode, IQuadNode, IQuadNode> Split()
        {
            return new Tuple<IQuadNode, IQuadNode, IQuadNode, IQuadNode>(DeepClone(), DeepClone(), DeepClone(), DeepClone());
        }

        /// <summary>
        /// Returns a deepclone of this object
        /// </summary>
        /// <returns></returns>
        public WeatherNode DeepClone()
        {
            return new WeatherNode()
            {
                Temperature = this.Temperature,
                Humidity = this.Humidity,
                Precipitation = this.Precipitation.Clone() as ICloneable,
                Pressure = this.Pressure
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Object ICloneable.Clone()
        {
            return DeepClone();
        }
    }
}