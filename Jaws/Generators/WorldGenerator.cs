using Jaws.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jaws.Generators
{
    public class WorldGenerator
    {
        const Int32 WorldFaceSize = 4 ^ 7;

        /// <summary>
        /// Generates a new world with 6 faces
        /// </summary>
        /// <returns></returns>
        public static CircularQuadTree<WeatherNode> Generate()
        {
            return new CircularQuadTree<WeatherNode>(
                WeatherNode.Generate(WorldGenerator.WorldFaceSize)
            );
        } 
    }
}
