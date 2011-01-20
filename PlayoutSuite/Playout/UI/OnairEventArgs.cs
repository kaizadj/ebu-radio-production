using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogListPrj;

namespace Playout.UI
{
    public class OnairEventArgs : EventArgs
    {
        public SlotItem onairslot {get; private set;}
        public OnairEventArgs(SlotItem slot)
        {
            this.onairslot = slot;
        }

    }
}
