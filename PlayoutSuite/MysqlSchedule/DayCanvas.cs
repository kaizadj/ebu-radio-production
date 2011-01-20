using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MysqlSchedule
{
    public class PeriodCanvas
    {
        public String name;
        public int size;
        public String id;

        public List<SliceCanvas> slices;
        
        public PeriodCanvas(String id, String name, int size)
        {
            this.id = id;
            this.name = name;
            this.size = size;
            this.slices = new List<SliceCanvas>();
        }
    }
}
