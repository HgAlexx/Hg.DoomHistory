using System;
using System.Collections.Generic;

namespace Hg.DoomHistory
{
    public class TimeStampComparer : IComparer<GameDetails>
    {
        public int Compare(GameDetails x, GameDetails y)
        {
            if (x == null && y == null)
                return 0;
            if (x == null)
                return -1;
            if (y == null)
                return 1;
            return DateTime.Compare(y.SaveDateTime, x.SaveDateTime);
        }
    }
}
