using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Interaction
{
    public enum EntryStatus
    { 
        Creating,
        Created
    }

    public class CacheEntry
    {
        public EntryStatus Status { get; set; }

        public object Value { get; set; }
    }
}
