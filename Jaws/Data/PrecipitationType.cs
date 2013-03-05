using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jaws.Data
{
    [Flags]
    public enum PrecipitationType
    {
        None = 0,

        // Types
        Drizzle = (1 << 0),
        Rain = (1 << 1),
        Hail = (1 << 2),
        Snow = (1 << 3),

        // Modifier
        FreezingDrizzle = (1 << 4),
        FreezingRain = (1 << 5),

        // Rare
        SnowPellets = (1 << 6),
        IcePellets = (1 << 7),
    }
}
