using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DS;
using LogListPrj.Exceptions;
using LogListPrj.DataItems;
//using MMBS.DataStructures;

namespace LogListPrj
{
    public class LogList : ILogList<SlotItem, Slice>
    {
        private LinkedList<Slice> islices;
        private TimeSpan iruntime;
        private DateTime istartdatetime;
        public DataTable datatable = null;

        public LogList(DateTime startdatetime)
        {
            this.islices = new LinkedList<Slice>();
            this.istartdatetime = startdatetime;
            this.iruntime = TimeSpan.Zero;
           // loadPlaylist(startdatetime);
            
        }
        public LogList() : this(DateTime.Now) { }

        public void loadPlaylist()//DateTime startdatetime)
        {
           // this.istartdatetime = startdatetime;
            //loadDistDummy(startdatetime, "D:\\UER\\MM\\playlist.xml");

            Console.WriteLine("PLAYLIST GENERATION!!");


            this.generateDataTable();
            datatable.RowDeleting += new DataRowChangeEventHandler(datatable_RowDeleting);
        }

        void datatable_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            Console.WriteLine("Removing element");
            if (e.Action == DataRowAction.Delete) //&& e.Row["type"] != "slice")
            {
                SlotItem slot = (SlotItem)e.Row["slot"];
                if (slot != null)
                {
                    if (slot.status != DS.SlotStatus.PLAYED)
                    {
                        slot.remove();
                    }
                }
            }
            
        }

        // Slices

        public Slice newSliceOnList(DateTime dt)
        {
            Slice newSlice = new Slice(dt);
            newSlice.node = this.islices.AddLast(newSlice);
            return newSlice;
        }

        private long newId(long i, DateTime day)
        {
            int WIDTH = 8;
            String s = "" + i;
            if (s.Length < WIDTH) s = s.PadLeft(WIDTH, '0');
            return long.Parse(day.ToString("yyyyMMdd") + "" + s);
        }

        public void addDataToSlice(DataItem item, Slice slice, long histid, DateTime scheduleddatetime)
        {
            //DateTime dt = slice.startdatetime+slice.runtime;
            //if(slice.slots.Count != 0) dt = slice.slots.Last.Value.airdatetime.AddMilliseconds(slice.slots.Last.Value.runtime.TotalMilliseconds);
            SlotItem slot = new SlotItem(histid, item, scheduleddatetime, new TimeMarker(item.runtime), slice);
            slot.node = this.islices.Last.Value.slots.AddLast(slot);
        }

        //DataTable



        public void print()
        {
            for (int i = 0; i < this.slices.Count; i++)
            {
                this.islices.ElementAt(i).print();
            }
        }


        public DataTable getDataTable()
        { return datatable; }

        private void generateDataTable()
        {
            if (datatable == null)
            {

                datatable = new DataTable();
                datatable.Columns.Add(new DataColumn("status", typeof(DS.SlotStatus)));
                datatable.Columns.Add(new DataColumn("label", typeof(string)));
                datatable.Columns.Add(new DataColumn("runtime", typeof(TimeSpan)));
                datatable.Columns.Add(new DataColumn("airdatetime", typeof(DateTime)));
                datatable.Columns.Add(new DataColumn("type", typeof(string)));
                datatable.Columns.Add(new DataColumn("filename", typeof(string)));
                datatable.Columns.Add(new DataColumn("slot", typeof(SlotItem)));
                datatable.Columns.Add(new DataColumn("slice", typeof(Slice)));
                

                for (int i = 0; i < this.slices.Count; i++)
                {
                    this.islices.ElementAt(i).getDataRows(datatable);
                    
                }
            }
        }


        //Properties
        
        public DateTime startdatetime
        {
            get
            {
                return istartdatetime;
            }
            set
            {
                this.istartdatetime = value;
            }
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



        public LinkedList<Slice> slices
        {
            get
            {
                return this.islices;
            }
            set
            {
                
            }
        }


        //Log Action
        public void refreshAirdatetime()
        {
            for (int i = 0; i < this.slices.Count; i++)
            {
                this.slices.ElementAt(i).refreshTiming();
            }
        }

        public SlotItem getScheduledSlot(DateTime instant)
        {
            for (int i = 0; i < this.slices.Count; i++)
            {
                Slice slice = this.slices.ElementAt(i);
                DateTime diffdatetime = slice.scheduleddatetime;
                TimeSpan druntime = slice.runtime;
                if (slice.node.Next != null && slice.node.Next.Value.scheduleddatetime <= instant)
                {
                    //SWITCH TO NEXT SLICE
                    //Console.WriteLine(slice.scheduleddatetime.ToLongTimeString()+"NEXT SLICE SWITCH " + slice.node.Next.Value.airdatetime.ToLongTimeString() + " " + diffdatetime.Add(druntime).ToLongTimeString());
                }
                else if (diffdatetime <= instant
                    && diffdatetime.Add(druntime) > instant)
                {
                    //Console.WriteLine("GOOD SLICE");
                    return slice.getScheduledSlot(instant);
                }

            }
            throw new NotFoundSlotException();

        }

        public SlotItem getAirSlot(DateTime instant)
        {
            for (int i = 0; i < this.slices.Count; i++)
            {
                Slice slice = this.slices.ElementAt(i);
                DateTime diffdatetime = slice.airdatetime;
                TimeSpan druntime = slice.runtime;
                if (slice.node.Next != null && slice.node.Next.Value.airdatetime <= instant)
                {
                    //SWITCH TO NEXT SLICE
                    //Console.WriteLine(slice.scheduleddatetime.ToLongTimeString()+"NEXT SLICE SWITCH " + slice.node.Next.Value.airdatetime.ToLongTimeString() + " " + diffdatetime.Add(druntime).ToLongTimeString());
                }
                else if (diffdatetime <= instant
                    && diffdatetime.Add(druntime) > instant)
                {
                    //Console.WriteLine("GOOD SLICE");
                    return slice.getAirSlot(instant);
                }
                //else
                //Console.WriteLine("NOT THIS SWITCH");

            }
            throw new NotFoundSlotException();
        }


        public SlotItem getSlotById(long id)
        {
            SlotItem slot = this.slices.First.Value.slots.First.Value;
            try
            {
                while (slot.uniqID != id)
                {
                    slot = slot.NextSlot;
                }
            }
            catch { }
            
            return slot;
        }

   
        public void skipUntilSlot(SlotItem current)
        {
            try
            {
                SlotItem c = current.PreviousSlot;
                while (c != null)
                {
                    c.status = SlotStatus.SKIPPED;
                    c = c.PreviousSlot;
                }

            }
            catch (KeyNotFoundException e)
            {

            }
        }
    }
}
