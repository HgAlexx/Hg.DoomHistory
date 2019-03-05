using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Hg.DoomHistory.Types;

namespace Hg.DoomHistory.Comparers
{
    public class TimeStampComparer : IComparer<GameDetails>
    {
        #region Fields & Properties

        private SortOrder _sortAscending;

        public SortOrder SortAscending
        {
            get => _sortAscending;
            set => _sortAscending = value;
        }

        #endregion

        #region Members

        public TimeStampComparer(SortOrder sortAscending)
        {
            _sortAscending = sortAscending;
        }

        public int Compare(GameDetails x, GameDetails y)
        {
            if (x == null && y == null)
            {
                return 0;
            }

            if (x == null)
            {
                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            if (_sortAscending == SortOrder.Ascending)
            {
                return DateTime.Compare(x.SaveDateTime, y.SaveDateTime);
            }

            return DateTime.Compare(y.SaveDateTime, x.SaveDateTime);
        }

        #endregion
    }
}