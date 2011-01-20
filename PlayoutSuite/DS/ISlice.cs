using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS
{
    public enum SlotStatus { ONAIR, PLAYED, SKIPPED, ERROR, READY, WAITING, AUTO, CLOSING };
    public enum SliceStatus { ONAIR, PLAYED, SKIPPED, READY, WAITING };
    public interface ISlice<QSlotItem>
    {
        DateTime startdatetime { get; set; }
        DateTime airdatetime { get; set; }
        TimeSpan runtime { get; set; }
        TimeSpan gaptonextslice { get; set; }
        SliceStatus status { get; set; }
        LinkedList<QSlotItem> slots { get; set; }
        //node
    }
}
