using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Hg.DoomHistory.Types;

namespace Hg.DoomHistory.Comparers
{
    public class PlayedTimeComparer : IComparer<GameDetails>
    {
        #region Fields & Properties

        private SortOrder _sortOrder;

        public SortOrder SortOrder
        {
            get => _sortOrder;
            set => _sortOrder = value;
        }

        #endregion

        #region Members

        public PlayedTimeComparer(SortOrder sortOrder)
        {
            _sortOrder = sortOrder;
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

            if (_sortOrder == SortOrder.Ascending)
            {
                return TimeSpan.Compare(x.PlayedTime, y.PlayedTime);
            }

            return TimeSpan.Compare(y.PlayedTime, x.PlayedTime);
        }

        #endregion
    }
}