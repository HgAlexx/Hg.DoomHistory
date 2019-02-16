using System;
using System.Collections.Generic;

namespace Hg.DoomHistory
{
    public class MapComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == null)
                return 1;
            if (y == null)
                return -1;
            return MapNameToIndex(x).CompareTo(MapNameToIndex(y));
        }

        public static int MapNameToIndex(string name)
        {
            int index = Array.IndexOf(MapData.MapNames, name) + 1;
            return index;
        }
        public static int MapDescENToIndex(string name)
        {
            int index = Array.IndexOf(MapData.MapsDescEN, name) + 1;
            return index;
        }
    }
}