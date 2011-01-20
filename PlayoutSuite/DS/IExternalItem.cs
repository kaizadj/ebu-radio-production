using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS
{
    enum ExternalType { LINEIN, STREAM };
    interface IExternalItem : IDataItem
    {
        ExternalType type { get; set; }
        Uri url { get; set; }
        int line { get; set; }
    }
}
