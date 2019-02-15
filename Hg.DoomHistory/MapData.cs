using System.Collections.Generic;

namespace Hg.DoomHistory
{
    public class MapData
    {
        public static readonly string[] Maps =
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

        public string Path { get; }

        public override string ToString()
        {
            string level = ("#" + MapComparer.MapNameToIndex(Name)).PadRight(3);
            return level + ": " + Name;
        }
    }
}