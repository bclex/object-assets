
using System;

namespace OA.Ultima.World.Input
{
    [Flags]
    public enum PickType
    {
        PickNothing = 0,
        PickObjects = 1,
        PickStatics = 2,
        PickGroundTiles = 4,
        PickEverything = PickObjects | PickStatics | PickGroundTiles
    }
}