using System.Collections.Generic;
using Hg.DoomHistory.Types;

namespace Hg.DoomHistory.Comparers
{
    public class MapComparer : IComparer<MapData>
    {
        #region Members

        public int Compare(MapData x, MapData y)
        {
            if (x == null && y == null)
            {
                return 0;
            }

            if (x == null)
            {
                return 1;
            }

            if (y == null)
            {
                return -1;
            }

            return x.LevelNumber.CompareTo(y.LevelNumber);
        }

        public static int MapNameToLevel(string name)
        {
            return MapData.MapsLevels.TryGetValue(name, out var index) ? index : 0;
        }

        #endregion
    }
}