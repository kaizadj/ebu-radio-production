using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MysqlSchedule
{
    public class SliceCanvasItem : SliceCanvas
    {
        public int position;
        public String idperioditem;
        public SliceCanvasItem(String idperioditem, int position, String id, String name, TimeSpan scheduleddt)
            : base(id, name, scheduleddt)
        {
            this.position = position;
            this.idperioditem = idperioditem;
        }
    }
}
