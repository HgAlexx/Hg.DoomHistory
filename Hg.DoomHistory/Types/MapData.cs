using System;
using System.Collections.Generic;
using System.IO;
using Hg.DoomHistory.Comparers;

namespace Hg.DoomHistory.Types
{
    public class MapData : IEquatable<MapData>
    {
        #region Fields & Properties

        public static Dictionary<string, int> MapsLevels = new Dictionary<string, int>
        {
            {@"game/sp/intro/intro", 1},
            {@"game/sp/resource_ops/resource_ops", 2},
            {@"game/sp/resource_ops_foundry/resource_ops_foundry", 3},
            {@"game/sp/surface1/surface1", 4},
            {@"game/sp/argent_tower/argent_tower", 5},
            {@"game/sp/blood_keep/blood_keep", 6},
            {@"game/sp/surface2/surface2", 7},
            {@"game/sp/bfg_division/bfg_division", 8},
            {@"game/sp/lazarus/lazarus", 9},
            {@"game/sp/lazarus_2/lazarus_2", 9},
            {@"game/sp/blood_keep_b/blood_keep_b", 10},
            {@"game/sp/blood_keep_c/blood_keep_c", 11},
            {@"game/sp/polar_core/polar_core", 12},
            {@"game/sp/titan/titan", 13}
        };

        public int LevelNumber { get; }

        public string Name { get; }

        public string NameInternal { get; }

        public string NameSafe { get; }

        #endregion

        #region Members

        public MapData(string name, string nameSafe, string nameInternal)
        {
            Name = name;
            NameSafe = nameSafe;
            NameInternal = nameInternal;
            LevelNumber = MapComparer.MapNameToLevel(NameInternal);

            foreach (char invalidPathChar in Path.GetInvalidPathChars())
            {
                NameSafe = NameSafe.Replace(invalidPathChar, ' ');
            }
        }

        public bool Equals(MapData other)
        {
            if (other == null)
            {
                return false;
            }

            int levelNumber = MapComparer.MapNameToLevel(other.NameInternal);
            return LevelNumber == levelNumber;
        }

        public override int GetHashCode()
        {
            return LevelNumber;
        }

        public override string ToString()
        {
            return ("#" + LevelNumber).PadRight(3) + ": " + Name;
        }

        #endregion
    }
}