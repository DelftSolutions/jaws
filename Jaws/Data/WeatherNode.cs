using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jaws.Data
{
    public class WeatherNode : IQuadNode, ICloneable
    {
        public Single Temperature { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public WeatherNode()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Tuple<IQuadNode, IQuadNode, IQuadNode, IQuadNode> Split()
        {
            return new Tuple<IQuadNode, IQuadNode, IQuadNode, IQuadNode>(DeepClone(), DeepClone(), DeepClone(), DeepClone());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public WeatherNode DeepClone()
        {
            return new WeatherNode()
            {

            };
        }

        Object ICloneable.Clone()
        {
            return DeepClone();
        }
    }
}