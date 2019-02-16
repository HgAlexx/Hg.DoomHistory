using System.Collections.Generic;

namespace Hg.DoomHistory
{
    public class MapData
    {
        public static readonly string[] MapsDescEN =
        {
            @"The UAC",
            @"Resource Operations",
            @"Foundry",
            @"Argent Facility",
            @"Argent Energy Tower",
            @"Kadingir Sanctum",
            @"Argent Facility (Destroyed)",
            @"Advanced Research Complex",
            @"Lazarus Labs",
            @"Titan's Realm",
            @"The Necropolis",
            @"VEGA Central Processing",
            @"Argent D'Nur"
        };

        public static readonly string[] MapNames =
        {
            @"game/sp/intro/intro",
            @"game/sp/resource_ops/resource_ops",
            @"game/sp/resource_ops_foundry/resource_ops_foundry",
            @"game/sp/surface1/surface1",
            @"game/sp/argent_tower/argent_tower",
            @"game/sp/blood_keep/blood_keep",
            @"game/sp/surface2/surface2",
            @"game/sp/bfg_division/bfg_division",
            @"game/sp/lazarus/lazarus",
            @"game/sp/blood_keep_b/blood_keep_b",
            @"game/sp/blood_keep_c/blood_keep_c",
            @"game/sp/polar_core/polar_core",
            @"game/sp/titan/titan"
        };

        public readonly List<GameDetails> Games = new List<GameDetails>();

        public MapData(string parentPath, string name)
        {
            Name = name;
            NameSafe = Name;

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
            int index = MapComparer.MapNameToIndex(NameInternal);
            if (index < 0)
                index = MapComparer.MapDescENToIndex(Name);
            string level = ("#" + index).PadRight(3);
            return level + ": " + Name;
        }
    }
}
