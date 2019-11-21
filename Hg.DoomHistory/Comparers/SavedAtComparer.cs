using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Hg.DoomHistory.Types;

namespace Hg.DoomHistory.Comparers
{
    public class SavedAtComparer : IComparer<GameDetails>
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

        public SavedAtComparer(SortOrder sortOrder)
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
                return DateTime.Compare(x.SavedAt, y.SavedAt);
            }

            return DateTime.Compare(y.SavedAt, x.SavedAt);
        }

        #endregion
    }
}