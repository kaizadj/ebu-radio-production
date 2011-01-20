using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS;
using System.ComponentModel;
using LogListPrj.DataItems;
using LogListPrj.Exceptions;

namespace LogListPrj
{
    public class SlotItem: INotifyPropertyChanged, ISlotItem<Slice, DataItem>
    {
        private long iID;
        private DateTime ischeduleddatetime;
        private DateTime iairdatetime;
        
        private DataItem idataitem;
        private SlotAirStatus iairstatus;
        
        private Slice iparent;
        public LinkedListNode<SlotItem> node;
        private TimeMarker itimemarker = new TimeMarker();
        public TimeMarker timemarker { get{ return (idataitem.dataitemtype == DataItemType.SONG)? ((DataSongItem)idataitem).timemarker:itimemarker; } }

       
        public SlotItem(long ID, DataItem dataitem, DateTime scheduleddatetime, TimeMarker timemarker, Slice parent)
        {
            this.iID = ID;
            this.idataitem = dataitem;
            this.ischeduleddatetime = scheduleddatetime;
            this.iairstatus = new SlotAirStatus();
            this.iparent = parent;
            this.node = null;
        }



        public DateTime airdatetime
        {
            get
            {
                DateTime retairdatetime = this.iairdatetime;

                if (this.idataitem.dataitemtype == DS.DataItemType.SYNC)
                {
                    DataSyncItem sync = this.idataitem as DataSyncItem;
                    if (sync.synctype == DS.SyncType.HARD)
                    {
                        this.iairdatetime = sync.scheduleddatetime;
                    }
                    else if (sync.synctype == DS.SyncType.SOFT)
                    {
                        ///TODO : COMPUTE AIR DATE TIME WHEN SOFT SYNC!
                        if (this.Parent.node.Previous != null)
                        {
                            Slice prev = this.Parent.node.Previous.Value;

                        }
                    }
                }
                if (this.iairdatetime == DateTime.MinValue) this.iairdatetime = this.scheduleddatetime;

                if (this.iairdatetime != retairdatetime) onPropertyChanged("airdatetime"); 
                return this.iairdatetime;
            }
            set
            {
                this.iairdatetime = value;
            }
        }

        public long uniqID { get { return iID; } }
        public DateTime scheduleddatetime { get { return this.ischeduleddatetime; } set { this.ischeduleddatetime = value; onPropertyChanged("scheduleddatetime"); } }
        
        public TimeSpan runtime { get { return this.idataitem.runtime; } set { this.idataitem.runtime = value; } }
        public Slice Parent { get { return this.iparent; } set { this.iparent = value; } }

        #region AirStatusProperties

        public Boolean fillsong
        {
            get { return this.iairstatus.fillsong; }
            set { this.iairstatus.fillsong = value; onPropertyChanged("fillsong"); }
        }
        public Boolean cutsong
        {
            get { return this.iairstatus.cutsong; }
            set { this.iairstatus.cutsong = value; onPropertyChanged("cutsong"); }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void onPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        #endregion

        public DataItem item { get { return idataitem; } set { idataitem = value; onPropertyChanged("item"); } }

        public void print()
        {
            if (this.item.dataitemtype == DS.DataItemType.SYNC)
            {
                Console.ForegroundColor = (((DataSyncItem)this.item).synctype == DS.SyncType.HARD) ? ConsoleColor.Red : ConsoleColor.Yellow;
            }

            Console.WriteLine(this.uniqID + "\t" + this.scheduleddatetime.ToLongTimeString() + "\t " + this.airdatetime.ToLongTimeString() + "\t " + (this.runtime) + " - " + this.item.label + "");
            Console.ForegroundColor = ConsoleColor.Gray;
        }



        #region properties
        public TimeSpan cuepoint
        {
            get
            {
                return this.timemarker.cue;
            }
            set
            {
                this.timemarker.cue = value; onPropertyChanged("timemarker");
            }
        }

        public TimeSpan nextpoint
        {
            get
            {
                return this.timemarker.next;
            }
            set
            {
                this.timemarker.next = value; onPropertyChanged("timemarker");
            }
        }

        public ChainMode chainmode
        {
            get
            {
                return this.iairstatus.chainmode;
            }
            set
            {
                this.iairstatus.chainmode = value; onPropertyChanged("chainmode");
            }
        }

        public SlotStatus status
        {
            get
            {
                return this.iairstatus.status;
            }
            set
            {
                if (this.iairstatus.status != value)
                {
                    this.iairstatus.status = value;
                    onPropertyChanged("status");
                }
            }
        }

        public Slice slice
        {
            get
            {
                return iparent;
            }
            set
            {
                iparent = value;
            }
        }

        #endregion

        internal SlotItem remove()
        {
            bool isSync = (this.item.dataitemtype == DS.DataItemType.SYNC);//IS A SYNCHRO? if yes refresh timing from previous slice (FILLSONG & CUTSONG refresh)
            LinkedList<SlotItem> l = this.node.List; // take slots
            l.Remove(this);
            LinkedListNode<Slice> scurrent = this.slice.node;
            bool stop = false;
            int i = 0;
            while (scurrent != null && !stop)
            {
                i++;
                if (scurrent.Value.slots.Count != 0)
                {
                    if (i!=1 && scurrent.Value.slots.First.Value.item.dataitemtype == DataItemType.SYNC && ((DataSyncItem)scurrent.Value.slots.First.Value.item).synctype == SyncType.HARD)
                    {
                        Console.WriteLine(i + " first hardsync");
                        //First HARD SYNC, check silence...
                        LinkedListNode<Slice> sprev = scurrent.Previous;
                        if (sprev != null)
                        {
                            
                        }
                        stop = true;
                    }
                        scurrent.Value.refreshTiming();
                }
                scurrent = scurrent.Next;
            }
            
            if (isSync && this.slice.node.Previous != null)
                this.slice.node.Previous.Value.refreshTiming();
                         

            return this;

        }

        public SlotItem PreviousSlot
        {
            get
            {
                if (node.Previous == null)
                    if (Parent.node.Previous != null)
                        return Parent.node.Previous.Value.slots.Last();
                    else
                        throw new NotFoundSlotException();
                else
                    return node.Previous.Value;
            }
            private set{}
        }
        public SlotItem NextSlot
        {
            get
            {
                if (node.Next == null)
                    if (Parent.node.Next != null)
                        return Parent.node.Next.Value.slots.First();
                    else
                        throw new NotFoundSlotException();
                else
                    return node.Next.Value;
            }
            private set { }
        }
    }
}
