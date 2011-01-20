using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS
{
    public enum SyncType { HARD, SOFT };
    public interface ISyncItem : IDataItem
    {
        SyncType synctype { get; set; }
    }
}
