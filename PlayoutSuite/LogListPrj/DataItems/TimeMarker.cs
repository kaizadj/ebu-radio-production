using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogListPrj.DataItems
{
    public class TimeMarker
    {
        private TimeSpan icue;
        private TimeSpan inext;
        private TimeSpan iintroramp;
        private TimeSpan iintrodeadline;
        private TimeSpan iinhook;
        private TimeSpan iouthook;
        private TimeSpan ifadein;
        private TimeSpan ifadeout;
        private TimeSpan ifadeoutend;

        public TimeMarker(TimeSpan cue, TimeSpan introdeadline, TimeSpan next) { reset(); this.icue = cue; this.iintrodeadline = introdeadline; this.inext = next; }
        public TimeMarker(TimeSpan cue, TimeSpan next) { reset(); this.icue = cue; this.inext = next; }
        public TimeMarker(TimeSpan next) { reset(); this.inext = next; }
        public TimeMarker(){ reset(); }
         

        private void reset(){
            this.icue           = TimeSpan.Zero;
            this.inext          = TimeSpan.Zero;
            this.iintroramp     = TimeSpan.Zero;
            this.iintrodeadline = TimeSpan.Zero;
            this.iinhook        = TimeSpan.Zero;
            this.iouthook       = TimeSpan.Zero;
            this.ifadein        = TimeSpan.Zero;
            this.ifadeout       = TimeSpan.Zero;
            this.ifadeoutend    = TimeSpan.Zero;
        }


        public TimeSpan duration
        {
            get { return this.next.Subtract(this.cue); }
        }
        public TimeSpan cue
        {
            get { return this.icue; }
            set { this.icue = value; }
        }
        public TimeSpan next
        {
            get { return this.inext; }
            set { this.inext = value; }
        }
        public TimeSpan ramp
        {
            get { return this.iintroramp;  }
            set { this.iintroramp = value; }
        }
        public TimeSpan intro
        {
            get { return this.iintrodeadline; }
            set { this.iintrodeadline = value; }
        }
        public TimeSpan fadein
        {
            get { return this.ifadein; }
            set { this.ifadein = value; }
        }
        public TimeSpan fadeout
        {
            get { return this.ifadeout; }
            set { this.ifadeout = value; }
        }
        public TimeSpan fadeoutend
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
        public TimeSpan inhook
        {
            get { return this.iinhook; }
            set { this.iinhook = value; }
        }
        public TimeSpan outhook
        {
            get { return this.iouthook; }
            set { this.outhook = value; }
        }

    }
}
