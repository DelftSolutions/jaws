using Jaws.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jaws.Generators
{
    public class WorldGenerator
    {
        /// <summary>
        /// Generates a new world with 6 faces
        /// </summary>
        /// <returns></returns>
        public static CircularQuadTree<WeatherNode> Generate()
        {
            return new CircularQuadTree<WeatherNode>( 
                WeatherNode.Generate( 0, 0, 1035 ),
                WeatherNode.Generate( 0, 0, 1035 ),
                WeatherNode.Generate( 0, 0, 1035 ),
                WeatherNode.Generate( 0, 0, 1035 ),
                WeatherNode.Generate( 0, 0, 1035 ),
                WeatherNode.Generate( 0, 0, 1035 )
            );
        } 
    }
}
