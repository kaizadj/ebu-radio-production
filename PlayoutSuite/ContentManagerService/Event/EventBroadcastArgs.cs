using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContentManagerService
{
    class EventBroadcastArgs:EventArgs
    {
        public EventBroadcastArgs(String slide, String date)
        {
            this.slide = slide;
            this.date = date;
        }

        public string slide = "";

        public string date = "";
    }
}
