
using System;
using System.Collections.Generic;

namespace MapWebSite.Types
{
    public class DuplicateKeyComparer<TKey>
                 :
              IComparer<TKey> where TKey : IComparable
    {
        #region IComparer<TKey> Members

        public int Compare(TKey x, TKey y)
        {
            int result = x.CompareTo(y);

            return result == 0 ? 1 : result;
        }

        #endregion
    }
}
