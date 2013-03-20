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
    public class WeatherNode : IQuadNode, ICloneable, IEnumerable<WeatherLayer>
    {
        public const Single GroundLayerHeight = 7f;

        /// <summary>
        /// Layers with weather in this air column
        /// </summary>
        public List<WeatherLayer> Layers { get; set; }

        /// <summary>
        /// The ground temperature in Kelvin
        /// </summary>
        public Single Temperature { get { return this.GroundLayer.Temperature; } }

        /// <summary>
        /// The ground Humidity in Saturation percentage
        /// </summary>
        public Single Humidity { get { return this.GroundLayer.Humidity; } } 

        /// <summary>
        /// The ground Airpressure in hectoPascal (hPa/millibar)
        /// </summary>
        public Single Pressure { get { return this.GroundLayer.Pressure; } }

        /// <summary>
        /// The area of this air column
        /// </summary>
        public Single Area { get; protected set; }

        /// <summary>
        /// The ground precipitation (Weather by Water Vapour)
        /// </summary>
        public PrecipitationType Precipitation { get { return this.GroundLayer.Precipitation; } }

        /// <summary>
        /// 
        /// </summary>
        public WeatherLayer GroundLayer { get { return this.Layers[0];  } }

        /// <summary>
        /// Constructor
        /// </summary>
        protected WeatherNode() 
        {
            this.Layers = new List<WeatherLayer>();
        }

        /// <summary>
        /// Generates a weathernode
        /// </summary>
        /// <param name="temperature">Temperature in Kelvin</param>
        /// <param name="humidity">Humidity in percentage</param>
        /// <param name="pressure">Air Pressure in hectoPascal</param>
        /// <param name="precipitation">Precipitation type</param>
        /// <returns>The generated weathernode</returns>
        public static WeatherNode Generate(Single area, Single temperature = 0, Single humidity = 0, Single pressure = 1305, PrecipitationType precipitation = PrecipitationType.None )
        {
            var node = new WeatherNode() { Area = area };

            // Ground Layer
            node.Layers.Add(WeatherLayer.Generate(GroundLayerHeight, temperature, humidity, pressure, precipitation));
            
            return node;
        }

        /// <summary>
        /// Splits this node in four smaller nodes
        /// </summary>
        /// <returns></returns>
        public IQuadNode[] Split()
        {
            var topleft = DeepClone();
            var topright = DeepClone();
            var bottomright = DeepClone();
            var bottomleft = DeepClone();

            topleft.Area = this.Area / 4;
            topright.Area = this.Area / 4;
            bottomright.Area = this.Area / 4;
            bottomleft.Area = this.Area / 4;

            return new IQuadNode[] { topleft, topright, bottomright, bottomleft };
        }

        /// <summary>
        /// Returns a deepclone of this object
        /// </summary>
        /// <returns>the clone</returns>
        public WeatherNode DeepClone()
        {
            var node = new WeatherNode();
            node.Area = this.Area;
            foreach (var layer in this.Layers)
                node.Layers.Add(layer.DeepClone());
            return node;
        }

        /// <summary>
        /// Clones the Object
        /// </summary>
        /// <returns>the clone</returns>
        Object ICloneable.Clone()
        {
            return DeepClone();
        }

        /// <summary>
        /// Gets the enumerator for this node
        /// </summary>
        /// <returns></returns>
        public IEnumerator<WeatherLayer> GetEnumerator()
        {
            return this.Layers.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator for this object
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Checks if this node is equal to another node
        /// </summary>
        /// <param name="obj">the other node</param>
        /// <returns>true if equal</returns>
        public override bool Equals(object obj)
        {
            var that = obj as WeatherNode;
            if (that == null)
                return false;

            // Check if layers are equal
            var that_enumerator = that.GetEnumerator();
            var that_has = that_enumerator.MoveNext();
            foreach (var layer in this)
            {
                if (!that_has)
                    return false;
                if (!layer.Equals(that_enumerator.Current))
                    return false;
                that_has = that_enumerator.MoveNext();
            }

            if (that_has)
                return false;

            // Check properties
            return this.Area == that.Area;
        }

        /// <summary>
        /// Gets the hashcode for this weathernode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (this.Area.GetHashCode() * 127) ^ 
                this.Layers.Sum(layer => layer.GetHashCode());
        }
    }
}