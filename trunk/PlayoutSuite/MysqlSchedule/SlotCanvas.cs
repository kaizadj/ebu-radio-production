using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MysqlSchedule
{
    public enum SlotCanvasType { UNKNOWN, SPECIFICITEM, CATEGORYITEM, SYNC, SLIDESLOAD };
    public class SlotCanvas
    {
        public int id;
        public int canvasid;
        public int sliceposition;
        public SlotCanvasType type;
        public String param1;
        public String param2;
        public String param3;
        public String label;


        public SlotCanvas(int id, int canvasid, int clockPosition, SlotCanvasType type, String param1, String param2, String param3, String label)
        {
            this.id = id;
            this.canvasid = canvasid;
            this.sliceposition = clockPosition;
            this.type = type;
            this.param1 = param1;
            this.param2 = param2;
            this.param3 = param3;
            this.label = label;
        }


    }
}
