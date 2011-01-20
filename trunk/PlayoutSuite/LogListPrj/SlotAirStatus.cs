using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS;

namespace LogListPrj
{
    public class SlotAirStatus
    {
        private ChainMode ichainmode;
        private SlotStatus istatus;
        private Boolean ifillsong;
        private Boolean icutsong;

        public SlotAirStatus()
        {
            this.ichainmode = ChainMode.AUTO;
            this.istatus = SlotStatus.WAITING;
            this.ifillsong = false;
            this.cutsong = false;
        }

        public SlotAirStatus(ChainMode chainmode)
        {
            this.ichainmode = chainmode;
            this.istatus = SlotStatus.WAITING;
            this.ifillsong = false;
            this.cutsong = false;
        }

        public ChainMode chainmode
        {
            get { return this.ichainmode; }
            set { this.ichainmode = value; }
        }
        
        public SlotStatus status
        {
            get { return this.istatus; }
            set { this.istatus = value; }
        }
        
        public Boolean fillsong
        {
            get { return this.ifillsong; }
            set { this.ifillsong = value; }
        }

        public Boolean cutsong
        {
            get { return this.icutsong; }
            set { this.icutsong = value; }
        }
    }
}
