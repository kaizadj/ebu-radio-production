using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS
{
    public enum ChainMode { AUTO, MANUAL, INTROLINKED };
    public interface ISlotItem<QSlice, QDataItem>
    {
        long uniqID { get; }
        DateTime scheduleddatetime { get; set; }
        DateTime airdatetime { get; set; }
        TimeSpan runtime { get; }
        TimeSpan cuepoint { get; set; }
        TimeSpan nextpoint { get; set; }
        QDataItem item { get; set; }
        ChainMode chainmode { get; set; }
        //node
        SlotStatus status { get; set; }
        QSlice slice { get; set; } //ref
    }
}
