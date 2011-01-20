using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS
{
    public enum DataItemType { SONG, LOGNOTE, SYNC, COMMAND, EXTERNAL, EMPTY };
    public interface IDataItem
    {
        long ID {get; set;}
        String label { get; set; }
        TimeSpan runtime { get; set; }
        String getField(String name);
        DataItemType dataitemtype { get; set; }
    }
}
