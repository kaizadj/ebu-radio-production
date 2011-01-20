using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioEngineDll.AudioTrack
{
    public class AudioTimeMarker
    {
        
        private TimeSpan cueStart;

        private TimeSpan cueNext;

        public AudioTimeMarker(TimeSpan start, TimeSpan next)
        {
            this.cueStart = start;
            this.cueNext = next;
        }
        public TimeSpan getDuration()
        {
            return cueNext.Subtract(cueStart);
        }
        public TimeSpan getStart() { return this.cueStart; }

        public TimeSpan getNext() { return this.cueNext; }

    }
}
