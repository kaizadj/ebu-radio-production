using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogListPrj.Exceptions;
using LogListPrj.DataItems;
using DS;
using System.ComponentModel;
using System.Data;

namespace LogListPrj
{
    public class Slice : ISlice<SlotItem>, INotifyPropertyChanged
    {

        #region ISlice Members


        private DateTime istartdatetime;
        private DateTime ischeduleddatetime;
        private DateTime iairdatetime;
        private TimeSpan iruntime;
        private TimeSpan igaptonextslice;
        private DS.SliceStatus istatus;
        private LinkedList<SlotItem> islots;
        public LinkedListNode<Slice> node;

        public Slice(DateTime lscheduleddatetime)
        {
            this.islots = new LinkedList<SlotItem>();
            this.ischeduleddatetime = lscheduleddatetime;
            this.istatus = SliceStatus.WAITING;

        }

        public Slice Previous{
            get{
                if(this.node.Previous!=null)
                return this.node.Previous.Value;
                else
                    return null;
            }
        }

        public String label
        {
            get
            {
                return "SLICE " + scheduleddatetime.ToShortDateString() + " - " + scheduleddatetime.ToShortTimeString();
            }
            set
            {
                //DO NOTHING
            }
        }

        public DateTime startdatetime
        {
            get
            {
                return this.istartdatetime;
            }
            set
            {
                istartdatetime = value;
            }
        }

        public DateTime enddatetime
        {
            get
            {
                throw new NotImplementedException();
                //return this.startdatetime.Add(this.runtime);
            }
        }
        public DateTime endairdatetime
        {
            get
            {
                return this.airdatetime.Add(this.runtime);
            }
        }

        public DateTime scheduleddatetime
        {
            get
            {
                if (this.slots.Count != 0)
                    return this.slots.ElementAt(0).scheduleddatetime;
                else
                {
                    new EmptySliceException();
                    return this.ischeduleddatetime;
                }
            }
        }

        public DateTime airdatetime
        {
            get
            {
                //Default: Scheduled time. (= FIRST SLICE OF A LIST)
                DateTime ret = this.scheduleddatetime;

                //HARD SYNC
                if (this.slots.Count != 0 && this.slots.First.Value.item.dataitemtype == DataItemType.SYNC
                    && ((DataSyncItem)this.slots.First.Value.item).synctype == SyncType.HARD)
                {
                    ret = ((DataSyncItem)this.slots.First.Value.item).scheduleddatetime;
                }
                //Previous slice exists
                else if (node.Previous != null && node.Previous.Value.slots.Count != 0)
                {
                    Slice previousSlice = node.Previous.Value;
                    SlotItem previousSlot = previousSlice.slots.Last.Value;

                    // SOFT SYNC
                    if (this.slots.Count != 0 && this.slots.First.Value.item.dataitemtype == DataItemType.SYNC
                        && ((DataSyncItem)this.slots.First.Value.item).synctype == SyncType.SOFT)
                    {
                        for (int i = previousSlice.slots.Count - 1; i >= 0; i--)
                        {
                            SlotItem slot = (previousSlice.slots.ElementAt(i));
                            if (slot.airdatetime <= this.scheduleddatetime)
                            {
                                ret = slot.airdatetime + slot.runtime;
                                break;
                            }
                        }


                    }
                    //NORMAL CHAINED SLICE
                    else
                    {
                        ret = previousSlot.airdatetime + previousSlot.runtime;
                    }
                }
                else { }

                return ret;
            }
            set
            {
                 this.iairdatetime = value;
            }
        }
        public Boolean isFillSong(SlotItem slot, Slice nextslice)
        {
            return nextslice.scheduleddatetime <= slot.airdatetime;
        }
        public void refreshTiming()
        {
            TimeSpan lruntime = TimeSpan.Zero;
            for (int i = 0; i < this.slots.Count; i++)
            {
                SlotItem slot = this.slots.ElementAt(i);

                slot.airdatetime = (i == 0) ? this.airdatetime : slot.node.Previous.Value.airdatetime + slot.node.Previous.Value.runtime;

                if (this.node.Next != null && this.node.Next.Value.airdatetime < slot.airdatetime + slot.runtime)
                {
                    //CUT BY AN HARD SYNC
                    slot.cutsong = true;

                    //CUT BY A SOFT SYNC
                    if (isFillSong(slot, this.node.Next.Value))
                    {
                        slot.fillsong = true;
                        slot.cutsong = false;
                        //willbeplayed = false;
                    }
                    else
                    {

                        slot.fillsong = false;
                    }
                }
                else
                {
                    slot.cutsong = false;
                    slot.fillsong = false;
                }

                lruntime = lruntime.Add(slot.runtime);
                slot.onPropertyChanged("airdatetime");
            }


            this.runtime = lruntime;
            onPropertyChanged("airdatetime");
            onPropertyChanged("runtime");

            onPropertyChanged("gaptonextslice");
        }




        public TimeSpan runtime
        {
            get
            {
                return this.iruntime;
            }
            set
            {
                this.iruntime = value;
            }
        }
        //scheduled, relative to only the concerned slice and the next one
        public TimeSpan gaptonextslice
        {
            get
            {
                if (this.node.Next != null && this.node.Next.Value != null)
                {
                    Slice next = this.node.Next.Value;
                    return this.scheduleddatetime.Add(this.runtime).Subtract(next.scheduleddatetime);
                }
                else return TimeSpan.Zero;
            }
            set
            {
                this.igaptonextslice = value;
            }

        }
        public TimeSpan airgaptonextslice
        {
            get
            {
                if (this.node.Next != null && this.node.Next.Value != null)
                {
                    Slice next = this.node.Next.Value;
                    return this.airdatetime.Add(this.runtime).Subtract(next.airdatetime);
                }
                else return TimeSpan.Zero;
            }
            set
            {
                this.igaptonextslice = value;
            }
        }

        public DS.SliceStatus status
        {
            get
            {
                return this.istatus;
            }
            set
            {
                this.istatus = value;
            }
        }

        #endregion

        public void print()
        {
            refreshTiming();
            if (this.node.List.First != this.node)
            {
                Slice prev = this.node.Previous.Value;
                Console.WriteLine("GAP : " + ((prev.gaptonextslice > TimeSpan.Zero) ? "+ " : "  ") + prev.gaptonextslice + "");
            }
            Console.WriteLine("\nNEW SLICE: sd" + this.scheduleddatetime + " ad" + this.airdatetime.ToLongTimeString() + " (" + this.runtime + ") " + this.status);
            Console.WriteLine("\n\tScheduledTime\tAirTime \truntime");
            for (int i = 0; i < this.slots.Count; i++)
            {
                if (this.slots.ElementAt(i).cutsong)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }
                else if (this.slots.ElementAt(i).fillsong)
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                }
                this.slots.ElementAt(i).print();
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            if (this.gaptonextslice < TimeSpan.Zero)
            {
                Console.WriteLine("###\t" + this.scheduleddatetime.Add(this.runtime).ToLongTimeString() + "\t " + this.airdatetime.Add(this.runtime).ToLongTimeString() + "\t " + (TimeSpan.Zero.Subtract(this.gaptonextslice)) + " - SILENCE");

            }
            Console.WriteLine("END SLICE: " + "\t\t" + this.endairdatetime.ToLongTimeString() + "  status:" + this.status + " \n");
        }

        #region ISlice<SlotItem> Members


        public LinkedList<SlotItem> slots
        {
            get
            {
                return this.islots;
            }
            set
            {
            }
        }

        #endregion
        /*
         * return Slot playing corresponding to the scheduleddatetime
         * if not found return null;
         */

        public SlotItem getScheduledSlot(DateTime datetime)
        {
            for (int i = 0; i < this.slots.Count; i++)
            {
                SlotItem slot = this.slots.ElementAt(i);
                if (slot.scheduleddatetime <= datetime && slot.scheduleddatetime + slot.runtime > datetime)
                {
                    return slot;
                }
            }
            return null;
        }
        /*
         * return Slot playing corresponding to the airdatetime
         * if not found return null;
         */

        public SlotItem getAirSlot(DateTime datetime)
        {

            for (int i = 0; i < this.slots.Count; i++)
            {
                SlotItem slot = this.slots.ElementAt(i);
                if (slot.airdatetime <= datetime && slot.airdatetime + slot.runtime > datetime)
                {
                    return slot;
                }
            }
            return null;
        }


        internal void getDataRows(DataTable dt)
        {

            var row = dt.NewRow();

            refreshTiming();
            if (this.node.List.First != this.node)
            {
                Slice prev = this.node.Previous.Value;
                //Console.WriteLine("GAP : " + ((prev.gaptonextslice > TimeSpan.Zero) ? "+ " : "  ") + prev.gaptonextslice + "");


                if (prev.endairdatetime < this.airdatetime)
                {
                    dt.Rows.Add(row);
                    row["type"] = "warning";
                    row["runtime"] = TimeSpan.Zero;

                    TimeSpan gap = TimeSpan.Zero.Subtract(prev.airgaptonextslice);
                    row["label"] = "!!! SILENCE !!! " + gap.ToString();
                    row["airdatetime"] = prev.endairdatetime.ToLongTimeString();

                }
                row = dt.NewRow();
            }

            dt.Rows.Add(row);
            row["type"] = "slice";
            row["runtime"] = this.runtime;
            row["label"] = "[SLICE] NEW";
            row["airdatetime"] = this.airdatetime;
            row["filename"] = "-";
            row["status"] = this.status;
            row["slice"] = this;
            row["slot"] = null;//new EmptySlotItem();

            // Console.WriteLine("\nNEW SLICE: sd" + this.scheduleddatetime + " ad" + this.airdatetime.ToLongTimeString() + " (" + this.runtime + ") " + this.status);
            //Console.WriteLine("\n\tScheduledTime\tAirTime \truntime");

            Random rdm = new Random();
            for (int i = 0; i < this.slots.Count; i++)
            {
                row = dt.NewRow();
                dt.Rows.Add(row);
                SlotItem slot = this.slots.ElementAt(i);
                if (slot.cutsong)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    row["type"] = "song"; // "cutsong";
                }
                else if (slot.fillsong)
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    //row["type"] = "fillsong";
                    row["type"] = "song";
                }
                else if (slot.item.dataitemtype.Equals(DataItemType.SYNC))
                {
                    if (((DataSyncItem)slot.item).synctype == SyncType.SOFT)
                    {
                        row["type"] = "timemarkersoft";
                    }
                    else if (((DataSyncItem)slot.item).synctype == SyncType.HARD)
                    {
                        row["type"] = "timemarkerhard";
                    }
                    else throw new NotSupportedException("SYNC UNDEFINED");
                }
                else if (slot.item.dataitemtype.Equals(DataItemType.SONG))
                {
                    row["type"] = "song";
                    row["filename"] = ((DataSongItem)slot.item).filename;
                }
                else if (slot.item.dataitemtype.Equals(DataItemType.LOGNOTE))
                {
                    //throw new NotImplementedException("LOGNOTE");
                    row["type"] = "lognote";
                }
                else
                {
                    throw new NotImplementedException();
                }
                //this.slots.ElementAt(i).print();
                row["runtime"] = slot.runtime;
                row["label"] = slot.item.label;
                row["airdatetime"] = slot.airdatetime;
                row["slot"] = slot;
                row["slice"] = null;
                row["status"] = this.status;


                Console.ForegroundColor = ConsoleColor.Gray;

            }
            if (this.gaptonextslice < TimeSpan.Zero)
            {
                //  Console.WriteLine("###\t" + this.scheduleddatetime.Add(this.runtime).ToLongTimeString() + "\t " + this.airdatetime.Add(this.runtime).ToLongTimeString() + "\t " + (TimeSpan.Zero.Subtract(this.gaptonextslice)) + " - SILENCE");

            }
            //Console.WriteLine("END SLICE: " + "\t\t" + this.endairdatetime.ToLongTimeString() + "  status:" + this.status + " \n");
        }

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
    }
}
