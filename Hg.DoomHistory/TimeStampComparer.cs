using System.Collections.Generic;

namespace Hg.DoomHistory
{
    public class TimeStampComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return string.CompareOrdinal(y, x);
        }
    }
}