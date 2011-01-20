using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MysqlSchedule
{
    public class SliceCanvas
    {



        //private DateTime istartdatetime;
        public String id;
        public String name;
        public TimeSpan scheduleddatetime;
        
       // LinkedList<SlotCanvas> slotsCanvas;
        
        public SliceCanvas(String id, String name, TimeSpan scheduleddt)
        {
            this.id = id;
            this.name = name;
            this.scheduleddatetime = scheduleddt;
        }

        
        
        
        
    }
}
