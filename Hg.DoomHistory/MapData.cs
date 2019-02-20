using System.Collections.Generic;

namespace Hg.DoomHistory
{
    public class MapData
    {
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

        public readonly List<GameDetails> Games = new List<GameDetails>();

        public MapData(string parentPath, string name, string nameSafe)
        {
            Name = name;
            NameSafe = nameSafe;

            foreach (char invalidPathChar in System.IO.Path.GetInvalidPathChars())
            {
                NameSafe = NameSafe.Replace(invalidPathChar, ' ');
            }

            Path = System.IO.Path.Combine(parentPath, NameSafe);
        }

        public string Name { get; }

        public string NameSafe { get; }

        public string NameInternal { get; set; }

        public string Path { get; }

        public override string ToString()
        {
            int levelNumber = MapComparer.MapNameToLevel(NameInternal);
            return ("#" + levelNumber).PadRight(3) + ": " + Name;
        }
    }
}