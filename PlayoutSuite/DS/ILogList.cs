using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS
{
    public interface ILogList<QSlotItem, QSlice>
    {
        LinkedList<QSlice> slices { get; set; }
        DateTime startdatetime { get; set; }
        TimeSpan runtime { get; set; }
    }
}
