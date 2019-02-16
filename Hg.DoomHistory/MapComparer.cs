using System;
using System.Collections.Generic;

namespace Hg.DoomHistory
{
    public class MapComparer : IComparer<MapData>
    {
        public int Compare(MapData x, MapData y)
        {
            if (x == null && y == null)
                return 0;
            if (x == null)
                return 1;
            if (y == null)
                return -1;
            return MapNameToLevel(x.NameInternal).CompareTo(MapNameToLevel(y.NameInternal));
        }

        public static int MapNameToLevel(string name)
        {
            return MapData.MapsLevels.TryGetValue(name, out var index) ? index : 0;
        }
    }
}