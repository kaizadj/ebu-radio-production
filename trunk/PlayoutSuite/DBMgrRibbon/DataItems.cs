using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DS;
using LogListPrj.DataItems;
using MysqlSchedule.DBlib;
using System.Windows;
using System.Windows.Controls;
using MysqlSchedule;

namespace DBMgrRibbon
{
    public class DataItems
    {
        private DataTable _datatable;
        public Dictionary<int, IDataItem> dataitems = new Dictionary<int, IDataItem>();
        public Dictionary<int, SliceCanvasItem> slicecanvas = new Dictionary<int, SliceCanvasItem>();
        public Dictionary<int, SlotCanvas> sliceitems = new Dictionary<int, SlotCanvas>();
        private DBengine dbengine;
        public DataItems(MainWindow win) { 
            this.mainwin = win; 
        }


        public void loadDataFromDB(ITEMFILTER filter = ITEMFILTER.NONE, String param=null)
        {
            try
            {
                dbengine = DBengineMySql.GetInstance();



                //TODO: CHECK CHANGES???

                dataitems.Clear();

                dbengine.connect("127.0.0.1", "root", "1234", "ebuplayout-dev2");

                _datatable = dbengine.listItems(filter, param);
                if (filter == ITEMFILTER.CATEGORY)
                {
                    for (int i = 0; i < _datatable.Rows.Count; i++)
                    {
                        if (_datatable.Rows[i]["type"].ToString() == "AUDIOFILE")
                        {
                            DataRow data = _datatable.Rows[i];
                            int id = (int)data["iddataitems"];
                            String artist = data["artist"].ToString();
                            String title = data["title"].ToString();
                            String filename = data["file"].ToString();
                            String radiovis1 = data["radiovis1"].ToString();
                            String radiovis2 = data["radiovis2"].ToString();
                            String radiovis3 = data["radiovis3"].ToString();
                            String radiovis4 = data["radiovis4"].ToString();
                            String radiovistxt = data["radiovistxt"].ToString();
                            String category = data["category"].ToString();
                            TimeSpan tmcue = TimeSpan.FromMilliseconds((int)data["tmcue"]);
                            TimeSpan tmnext = TimeSpan.FromMilliseconds((int)data["tmnext"]);
                            if (tmnext == TimeSpan.Zero)
                            {
                                tmnext = TimeSpan.FromMilliseconds((int)data["runtime"]);
                            }

                            DataSongItem song = new DataSongItem(id, artist, title, filename, new TimeMarker(tmcue, tmnext));
                            song.category = category;

                            song.setField("radiovis1", radiovis1);
                            song.setField("radiovis2", radiovis2);
                            song.setField("radiovis3", radiovis3);
                            song.setField("radiovis4", radiovis4);
                            song.setField("radiovistxt", radiovistxt);

                            dataitems.Add(id, song);
                        }
                    }
                }
                else if(filter == ITEMFILTER.CANVASPERIOD){
                    slicecanvas.Clear();
                    for (int i = 0; i < _datatable.Rows.Count; i++)
                    {
                        DataRow data = _datatable.Rows[i];

                        SliceCanvasItem c = new SliceCanvasItem(data["idcanvasperiod_items"].ToString(), (int)data["position"],  data["idslice"].ToString(), data["name"].ToString(), TimeSpan.FromMilliseconds(0));
                        
                        slicecanvas.Add(Int32.Parse(c.idperioditem), c);
                    }
                }
                else if (filter == ITEMFILTER.CANVASSLICE)
                {
                    sliceitems.Clear();
                    for (int i = 0; i < _datatable.Rows.Count; i++)
                    {
                        DataRow data = _datatable.Rows[i];

                        SlotCanvas slot = new SlotCanvas(Int32.Parse(data["idcanvasitem"].ToString()), Int32.Parse(data["idcanvasslice"].ToString()), Int32.Parse(data["clockposition"].ToString()), getSlotCanvasType(data["type"]), data["param1"].ToString(), data["param2"].ToString(), data["param3"].ToString(), data["label"].ToString());

                        sliceitems.Add(slot.id, slot);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            

        }

        private SlotCanvasType getSlotCanvasType(object p)
        {
            switch (p.ToString())
            {
                case "SPECIFICITEM":
                    return SlotCanvasType.SPECIFICITEM;
                case "CATEGORYITEM":
                    return SlotCanvasType.CATEGORYITEM;
                case "SYNC":
                    return SlotCanvasType.SYNC;
                case "SLIDESLOAD":
                    return SlotCanvasType.SLIDESLOAD;
                default:
                    return SlotCanvasType.UNKNOWN;

            }
        }
        public DataTable getDataTable()
        {
            
            return _datatable;
        }

        public DataTable getUIDataTable()
        {
            
            DataTable datatable = new DataTable();
            if (datatable.Columns.Count == 0)
            {
                datatable.Columns.Add(new DataColumn("id", typeof(Int32)));
                datatable.Columns.Add(new DataColumn("label", typeof(string)));
                datatable.Columns.Add(new DataColumn("artist", typeof(String)));
                datatable.Columns.Add(new DataColumn("runtime", typeof(TimeSpan)));
                datatable.Columns.Add(new DataColumn("category", typeof(String)));
                datatable.Columns.Add(new DataColumn("radiovis", typeof(RadioVisItems)));
                datatable.Columns.Add(new DataColumn("filename", typeof(string)));
                datatable.Columns.Add(new DataColumn("data", typeof(DataItem)));

            }
            datatable.Clear();
            for (int i = 0; i < this.dataitems.Count; i++)
            {
                DataSongItem item = (DataSongItem)this.dataitems.ElementAt(i).Value;
                var row = datatable.NewRow();
                row["id"] = item.ID;
                /*row["label"] = item.label;
                row["artist"] = item.artist;
                row["runtime"] = item.runtime;
                row["category"] = item.getField("category");*/
                row["radiovis"] = RadioVisItems.getFromDataItem(item);
               // row["filename"] = item.filename;
                row["data"] = item;
                datatable.Rows.Add(row);
            }

            datatable.RowChanged += new DataRowChangeEventHandler(datatable_RowChanged);
            return datatable;
        }

        public DataTable getUIDataTableCanvasPeriod()
        {

            DataTable datatable = new DataTable();
            if (datatable.Columns.Count == 0)
            {
                datatable.Columns.Add(new DataColumn("id", typeof(string)));
                datatable.Columns.Add(new DataColumn("label", typeof(string)));
                datatable.Columns.Add(new DataColumn("position", typeof(int)));
                //datatable.Columns.Add(new DataColumn("data", typeof(SliceCanvas)));
            }
            datatable.Clear();
            for (int i = 0; i < this.slicecanvas.Count; i++)
            {
                SliceCanvasItem item = this.slicecanvas.ElementAt(i).Value;
                var row = datatable.NewRow();
                row["id"] = item.idperioditem;
                row["label"] = item.name;
                row["position"] = item.position;
                //row["data"] = item;
                datatable.Rows.Add(row);
            }

            datatable.RowChanged += new DataRowChangeEventHandler(datatable_RowChanged);
            return datatable;
        }

        public DataTable getUIDataTableCanvasSlice()
        {

            DataTable datatable = new DataTable();
            if (datatable.Columns.Count == 0)
            {
                datatable.Columns.Add(new DataColumn("id", typeof(int)));
                datatable.Columns.Add(new DataColumn("type", typeof(SlotCanvasType)));
                datatable.Columns.Add(new DataColumn("sliceposition", typeof(int)));
                datatable.Columns.Add(new DataColumn("param1", typeof(string)));
                datatable.Columns.Add(new DataColumn("param2", typeof(string)));
                datatable.Columns.Add(new DataColumn("param3", typeof(string)));
                datatable.Columns.Add(new DataColumn("label", typeof(string)));
                datatable.Columns.Add(new DataColumn("slot", typeof(SlotCanvas)));
            }
            datatable.Clear();

            for (int i = 0; i < this.sliceitems.Count; i++)
            {
                SlotCanvas slot = this.sliceitems.ElementAt(i).Value;
                var row = datatable.NewRow();
                row["id"] = slot.id;
                row["type"] = slot.type;
                row["sliceposition"] = slot.sliceposition;
                row["param1"] = slot.param1;
                row["param2"] = slot.param2;
                row["param3"] = slot.param3;
                row["label"] = slot.label;
                row["slot"] = slot;
                datatable.Rows.Add(row);
            }

            datatable.RowChanged += new DataRowChangeEventHandler(datatable_RowChanged);
            return datatable;
        }

        void datatable_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Change)
            {
                Console.WriteLine(sender + " "+ sender.GetType());
                if (sender == mainwin.in_category)
                {
                    e.Row["category"] = ((ComboBoxItem)mainwin.in_category.SelectedItem).Tag;
                }
                else
                {
                    Console.WriteLine("item " + e.Row["id"] + " as changed.");
                }
                
            }
        }

        public Dictionary<String, DataItemCategory> getCategoryList()
        {
            return dbengine.listCategories();
        }

        public List<String> getCategoryTypesList()
        {
            return dbengine.listCategoryTypes();
        }

        public MainWindow mainwin;

        public List<PeriodCanvas> listPeriodCanvas()
        {
            return dbengine.listPeriodCanvas();
        }

        public List<SliceCanvas> listSliceCanvas()
        {
            return dbengine.listSliceCanvas();
        }

        public int newSlice(string p)
        {
            if (p != "")
            {
                return dbengine.newSlice(p);
            }
            else return -1;
        }

        public void removeSlice(int p)
        {
            dbengine.removeSlice(p);
        }

        public void removePeriod(int p)
        {
            dbengine.removePeriod(p);
        }
    }
}
