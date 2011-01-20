using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS
{
    public interface ISongItem : IDataItem
    {
        String filename { get; set; }
        String title { get; set; }
        String artist { get; set; }
        TimeSpan cuepoint { get; set; }
        TimeSpan nextpoint { get; set; }
        String category { get; set; }
    }
}
