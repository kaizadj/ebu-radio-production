using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using LogListPrj;
using LogListPrj.DataItems;

namespace MysqlSchedule.DBlib
{
    //SingleTon
    public class DBengineMySql : DBengine
    {

        MySqlConnection connection;
        MySqlDataAdapter adapter;
        private static DBengineMySql _UniqInstance = null;

        private DBengineMySql() {  }

        public Boolean isConnected()
        {
            if (connection == null)
                Console.WriteLine("[MYSQL ERROR] Use connect method first!");

            if (connection.State != ConnectionState.Open)
            {
                try
                {
                    this.connection.Open();
                    return true;
                }
                catch (Exception excp)
                {
                    Console.WriteLine("Error opening connection" +
                        " to the sql server. Error: " + excp.Message);
                }
                return false;
            }
            return true;
        }

        public static DBengineMySql GetInstance()
        {
            //creer une nouvelle instance s il n en existe pas deja une autre
            if (_UniqInstance == null)
                return _UniqInstance = new DBengineMySql();
            else
                return _UniqInstance;
        }

        public void connect(string host, string username, string password)
        {
            if (password != "")
            {
                throw new NotImplementedException();
            }
            string strConnection = "Server=" + host + ";Username=" + username + ";";
            connection = new MySqlConnection(strConnection);
        }

        public void connect(string host, string username, string password, string database)
        {
          /*  if (password != "")
            {
                throw new NotImplementedException();
            }*/
            string strConnection = "Server=" + host + ";Username=" + username + ";pwd="+password+";database=" + database;
            connection = new MySqlConnection(strConnection);
        }

        public System.Data.DataTable listItems(ITEMFILTER filter, String param)
        {
            try
            {
                string query = "SELECT dataitems.* FROM dataitems";

                    if (filter == ITEMFILTER.CATEGORY)
                    {
                        query += " WHERE category='" + param + "';";
                    }
                    else if (filter == ITEMFILTER.DATAITEMID)
                    {
                        query += " WHERE iddataitems='" + param + "';";
                    }
                    else if (filter == ITEMFILTER.CANVASSLICE)
                    {
                        query = "SELECT * FROM canvasitem";
                        if (param != null)
                        {
                            query += " WHERE idcanvasslice='" + param + "' ORDER By clockposition";
                        }
                    }
                    else if (filter == ITEMFILTER.CANVASPERIOD)
                    {
                        query = "SELECT * FROM canvasslice, canvasperiod_items"; 
                        if (param != null)
                        {
                            query += " WHERE idcanvasslice=idslice AND idperiod = '" + param + "' ORDER by position asc";
                        }
                    }

                //prepare adapter to run query
                adapter = new MySqlDataAdapter(query, connection);
                DataSet DS = new DataSet();
                //get query results in dataset
                adapter.Fill(DS);


                return DS.Tables[0];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public void newItem(Dictionary<DBfield, string> values)
        {

            List<String> k = new List<string>();
            List<String> v = new List<string>();

            foreach (KeyValuePair<DBfield, string> p in values)
            {
                k.Add(p.Key.name);
                v.Add(p.Value);
            }


            String cmd = "";
            cmd += "INSERT INTO `dataitems` (`datatype`, `label`, `runtime`) VALUES('SONG', '" + values.ToList().Where(p => p.Key.name == "artist").First().Value+" - " + values.ToList().Where(p => p.Key.name == "title").First().Value+"', 0);";

            /*cmd += "(`" + String.Join<String>("`, `", k) + "`) ";
            cmd += "VALUES ('" + String.Join("', '", v) + "');";*/
            MySqlCommand c = new MySqlCommand(cmd, connection);
            c.ExecuteNonQuery();

        }

        public void modifyItem(int iditem, Dictionary<DBfield, string> values)
        {
            throw new NotImplementedException();
        }


        public void Update(DataTable DTItems)
        {
            this.adapter.Update(DTItems);
        }



        public DBfield getField(int szid)
        {
            DBfield f = null;

            //Create Command
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM datafields WHERE `iddatafield` = '" + szid + "'", connection);
            //Create a data reader and Execute the command
            MySqlDataReader dataReader = cmd.ExecuteReader();

            //Read the data and store them in the list
            dataReader.Read();

            f = new DBfield(dataReader.GetInt32(dataReader["iddatafield"].ToString()), dataReader.GetString(dataReader["name"].ToString()));

            //close Data Reader
            dataReader.Close();

            return f;


        }

        public DBfield getField(String szname)
        {
            DBfield f = null;

            //Create Command
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM datafields WHERE `name` = '" + szname + "'", connection);
            //Create a data reader and Execute the command
            MySqlDataReader dataReader = cmd.ExecuteReader();

            //Read the data and store them in the list
            dataReader.Read();

            f = new DBfield(dataReader.GetInt32(dataReader["iddatafield"].ToString()), dataReader.GetString(dataReader["name"].ToString()));

            //close Data Reader
            dataReader.Close();

            return f;


        }
        public void newItem(Dictionary<string, string> values)
        {
            throw new NotImplementedException();
        }

        List<DBfield> DBengine.getFields()
        {
            List<DBfield> list = new List<DBfield>();

            Console.WriteLine("state : " + connection.State);
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM datafields", connection);

            //Create a data reader and Execute the command
            MySqlDataReader dataReader = cmd.ExecuteReader();

            //Read the data and store them in the list
            while (dataReader.Read())
            {
                int id = dataReader.GetInt32("iddatafield");
                String name = dataReader.GetString("name");
                list.Add(new DBfield(id, name));
            }

            dataReader.Close();

            return list;
        }



        public Dictionary<string, DataItemCategory> listCategories()
        {

            Dictionary<String, DataItemCategory> categories = new Dictionary<string, DataItemCategory>();

            isConnected();
            Console.WriteLine("state : " + connection.State);
            //Create Command
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM `dataitems_category`;", connection);
            //Create a data reader and Execute the command
            MySqlDataReader dataReader = cmd.ExecuteReader();
            Console.WriteLine(dataReader.FieldCount);
            //Read the data and store them in the list
            while(dataReader.Read()){
                int id = dataReader.GetInt32("id");
                String name = dataReader.GetString("name");
                String shortname = dataReader.GetString("shortname");
                String type = dataReader.GetString("type");

                DataItemCategory categ = new DataItemCategory(id, name, shortname, type);
                categories.Add(categ.shortname, categ);
                Console.WriteLine("categ: " + categ.shortname);
                
            }

            //close Data Reader
            dataReader.Close();

            return categories;
        }
        public List<String> listCategoryTypes()
        {

            List<String> types = new List<string>();

            isConnected();
            Console.WriteLine("state : " + connection.State);
            //Create Command
            MySqlCommand cmd = new MySqlCommand("SELECT DISTINCT `type` FROM `dataitems_category`;", connection);
            //Create a data reader and Execute the command
            MySqlDataReader dataReader = cmd.ExecuteReader();
            Console.WriteLine(dataReader.FieldCount);
            //Read the data and store them in the list
            while (dataReader.Read())
            {
                String type = dataReader.GetString("type");
                types.Add(type);
                Console.WriteLine("categ: " + type);

            }

            //close Data Reader
            dataReader.Close();

            return types;
        }
        public List<PeriodCanvas> listPeriodCanvas()
        {
            List<PeriodCanvas> periods = new List<PeriodCanvas>();

            isConnected();
            //Create Command
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM `canvasperiod`;", connection);
            //Create a data reader and Execute the command
            MySqlDataReader dataReader = cmd.ExecuteReader();
            Console.WriteLine(dataReader.FieldCount);
            //Read the data and store them in the list
            while (dataReader.Read())
            {
                String name = dataReader.GetString("name");
                int size = dataReader.GetInt32("size");
                String id = dataReader.GetString("idcanvasperiod");
                PeriodCanvas p = new PeriodCanvas(id.ToString(), name, size);
                periods.Add(p);
                

            }

            //close Data Reader
            dataReader.Close();

            return periods;
        }


        public List<SliceCanvas> listSliceCanvas()
        {
            List<SliceCanvas> slices = new List<SliceCanvas>();

            isConnected();
            //Create Command
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM `canvasslice` ORDER by name;", connection);
            //Create a data reader and Execute the command
            MySqlDataReader dataReader = cmd.ExecuteReader();
            Console.WriteLine(dataReader.FieldCount);
            //Read the data and store them in the list
            while (dataReader.Read())
            {
                String name = dataReader.GetString("name");
                Int32 id = dataReader.GetInt32("idcanvasslice");

                SliceCanvas s = new SliceCanvas(id.ToString(), name, TimeSpan.FromMilliseconds(0));
                slices.Add(s);


            }

            //close Data Reader
            dataReader.Close();

            return slices;
        }


        public int newSlice(string p)
        {
            isConnected();

            MySqlCommand cmd = new MySqlCommand("INSERT INTO `canvasslice`(`name`) VALUES('"+p+"');", connection);
            cmd.ExecuteNonQuery();
            return 0;

        }
        public void removeSlice(int id)
        {
            isConnected();
            String q = "DELETE FROM `canvasslice` WHERE idcanvasslice='" + id + "';";
            Console.WriteLine(q);
            MySqlCommand cmd = new MySqlCommand(q, connection);
            cmd.ExecuteNonQuery();
            
        }


        public void addNewCanvasItem(int canvasId, int slicePosition, SlotCanvasType slotCanvasType, string param1, string param2, string param3, string label)
        {
            isConnected();
            String q = "INSERT INTO `canvasitem` VALUES(NULL, '" + canvasId + "', '" + slicePosition + "','" + slotCanvasType.ToString() + "','" + param1 + "','" + param2 + "','" + param3 + "','" + label + "');";
            Console.WriteLine(q);
            MySqlCommand cmd = new MySqlCommand(q, connection);
            cmd.ExecuteNonQuery();
        }

        public void modifyCanvasItem(int canvasId, int slicePosition, int itemId, SlotCanvasType slotCanvasType, string param1, string param2, string param3, string label)
        {
            isConnected();
            String q = "UPDATE `canvasitem` SET type='" + slotCanvasType.ToString() + "', param1='" + param1 + "', param2='" + param2 + "', param3='" + param3 + "', label='" + label + "' WHERE idcanvasitem='"+itemId+"';";
            Console.WriteLine(q);
            MySqlCommand cmd = new MySqlCommand(q, connection);
            cmd.ExecuteNonQuery();
        }

        public void moveDownCanvasSlot(int currentSlice, int selectedId, int nextId, int currentPos)
        {
            isConnected();
            String q = "UPDATE `canvasitem` SET `clockPosition`='" + (currentPos) + "' WHERE `idcanvasitem`='" + nextId + "';\nUPDATE `canvasitem` SET `clockPosition`='" + (currentPos+1) + "' WHERE `idcanvasitem`='" + selectedId + "';";
            Console.WriteLine(q);
            MySqlCommand cmd = new MySqlCommand(q, connection);
            cmd.ExecuteNonQuery();
        }

        public void moveUpCanvasSlot(int p, int selectedId, int nextId, int currentPos)
        {
            isConnected();
            String q = "UPDATE `canvasitem` SET `clockPosition`='" + (currentPos) + "' WHERE `idcanvasitem`='" + nextId + "';\nUPDATE `canvasitem` SET `clockPosition`='" + (currentPos - 1) + "' WHERE `idcanvasitem`='" + selectedId + "';";
            Console.WriteLine(q);
            MySqlCommand cmd = new MySqlCommand(q, connection);
            cmd.ExecuteNonQuery();

        }

        public void removeFromSlice(int selectedId)
        {
            isConnected();
            String q = "DELETE FROM `canvasitem` WHERE `idcanvasitem`='" + selectedId + "';";
            Console.WriteLine(q);
            MySqlCommand cmd = new MySqlCommand(q, connection);
            cmd.ExecuteNonQuery();
        }

        public List<DateTime> getCalendarScheduledRanges()
        {
           /* isConnected();
            String q = "DELETE FROM `canvasitem` WHERE `idcanvasitem`='" + selectedId + "';";
            Console.WriteLine(q);
            MySqlCommand cmd = new MySqlCommand(q, connection);
            cmd.ExecuteNonQuery();*/
            throw new NotImplementedException();
        }

        public LogList loadLogList(DateTime start, DateTime end)
        {

            isConnected();
            String q = "SELECT * FROM scheduledslice ";

            LogList log = new LogList();


            //log.slices.AddLast();

            log.refreshAirdatetime();
            return log;
        }

        public void removeFromSlicePeriod(int selectedId, String currentPeriod)
        {
            isConnected();
            String q = "DELETE FROM `canvasperiod_items` WHERE idcanvasperiod_items='" + selectedId + "';";// AND idperiod='"+currentPeriod+"';";
            Console.WriteLine(q);
            MySqlCommand cmd = new MySqlCommand(q, connection);
            cmd.ExecuteNonQuery();
        }


        public void removePeriod(int id)
        {
            isConnected();
            String q = "DELETE FROM `canvasperiod` WHERE idcanvasperiod='" + id + "';\n";
            q += "DELETE FROM `canvasperiod_items` WHERE idperiod='" + id + "';";
            Console.WriteLine(q);
            MySqlCommand cmd = new MySqlCommand(q, connection);
            cmd.ExecuteNonQuery();
        }

        public void moveDownCanvasSlice(int p, int selectedId, int nextId, int currentPos)
        {
            isConnected();
            String q = "UPDATE `canvasperiod_items` SET `position`='" + (currentPos) + "' WHERE `idcanvasperiod_items`='" + nextId + "';\n";
            q += "UPDATE `canvasperiod_items` SET `position`='" + (currentPos + 1) + "' WHERE `idcanvasperiod_items`='" + selectedId + "';";
            Console.WriteLine(q);
            MySqlCommand cmd = new MySqlCommand(q, connection);
            cmd.ExecuteNonQuery();
        }

        public void moveUpCanvasSlice(int p, int selectedId, int nextId, int currentPos)
        {
            isConnected();
            String q = "UPDATE `canvasperiod_items` SET `position`='" + (currentPos) + "' WHERE `idcanvasperiod_items`='" + nextId + "';\n";
            q += "UPDATE `canvasperiod_items` SET `position`='" + (currentPos - 1) + "' WHERE `idcanvasperiod_items`='" + selectedId + "';";
            Console.WriteLine(q);
            MySqlCommand cmd = new MySqlCommand(q, connection);
            cmd.ExecuteNonQuery();
        }

        public void addSliceToPeriod(int periodId, int position, string sliceId)
        {
            isConnected();
            String q = "INSERT INTO `canvasperiod_items` VALUES(NULL, '" + periodId + "', '" + sliceId + "','" + position + "');";
            Console.WriteLine(q);
            MySqlCommand cmd = new MySqlCommand(q, connection);
            cmd.ExecuteNonQuery();
        }

        public Boolean isScheduled(DateTime date)
        {
            isConnected();
            String q = "SELECT COUNT(*) FROM `scheduledslices` WHERE scheduleddatetime = '"+date.ToFileTime()+"'";
            Console.WriteLine(q);
            MySqlCommand cmd = new MySqlCommand(q, connection);
            Int32 res = Int32.Parse(cmd.ExecuteScalar().ToString());
            return (res!=0);
            
        }

        public int scheduleNewSlice(string nameslice, DateTime scheduleddatetime)
        {
            isConnected();
            String q = "INSERT INTO `scheduledslices` (`scheduleddatetime`) VALUES ('" + scheduleddatetime.ToFileTime() + "');";
            Console.WriteLine(q);
            MySqlCommand cmd = new MySqlCommand(q, connection);
            cmd.ExecuteNonQuery();

            isConnected();
            q = "SELECT idscheduled FROM `scheduledslices` WHERE scheduleddatetime = '" + scheduleddatetime.ToFileTime() + "'";
            Console.WriteLine(q);
            MySqlCommand cmd2 = new MySqlCommand(q, connection);
            Int32 res = Int32.Parse(cmd2.ExecuteScalar().ToString());
            return res;
        }

        public void scheduleNewSyncToSlice(int idschedslice, int position, DateTime scheduleddate, string synctype, string synctime)
        {
            isConnected();
            String q = "INSERT INTO `scheduledslots` (`idscheduledslice`, `position`, `type`, `iddataitem`, `param`, `label`, `scheduleddatetime`) VALUES ('"+idschedslice+"', '"+position+"', 'SYNC', '-1', '"+synctype+"', 'SYNC "+synctype+"', '"+scheduleddate.ToFileTime()+"');";
            Console.WriteLine(q);
            MySqlCommand cmd = new MySqlCommand(q, connection);
            cmd.ExecuteNonQuery();
        }
        public DateTime scheduleNewSlidesLoad(int idschedslice, int position, DateTime scheduleddate, string slides)
        {
            isConnected();
            String q = "INSERT INTO `scheduledslots` (`idscheduledslice`, `position`, `type`, `iddataitem`, `param`, `label`, `scheduleddatetime`) VALUES ('" + idschedslice + "', '" + position + "', 'LOGNOTE', '-1', '@@@SLIDESLOAD-" + slides.Replace(',', '|') + "', '@@@SLIDESLOAD-" + slides.Replace(',', '|') + "', '" + scheduleddate.ToFileTime() + "');";
            Console.WriteLine(q);
            MySqlCommand cmd = new MySqlCommand(q, connection);
            cmd.ExecuteNonQuery();
            return scheduleddate;
        }
        public DateTime scheduleNewSpecificItemToSlice(int idschedslice, int position, DateTime scheduleddate, string iddataitem)
        {
            isConnected();
            String q = "INSERT INTO `scheduledslots` (`idscheduledslice`, `position`, `type`, `iddataitem`, `param`, `label`, `scheduleddatetime`) VALUES ('" + idschedslice + "', '" + position + "', 'AUDIO', '"+iddataitem+"', '-1', '', '" + scheduleddate.ToFileTime() + "');";
            Console.WriteLine(q);
            MySqlCommand cmd = new MySqlCommand(q, connection);
            cmd.ExecuteNonQuery();
            return scheduleddate.AddMinutes(3);
        
        }

        public DateTime scheduleNewCategoryItemToSlice(int idschedslice, int pos, DateTime scheduleddate, string categoryshortname)
        {
            DBDataItem item = getDataItemFromCategoryShortName(categoryshortname);
            return scheduleNewSpecificItemToSlice(idschedslice, pos, scheduleddate, item.id.ToString());
        }



        private DBDataItem getDataItemFromCategoryShortName(string categoryshortname)
        {
            //int idcateg = getCategoryIdFromShortName(categoryshortname);

            DBDataItem ret=null;

            isConnected();
            String q = "SELECT * FROM `dataitems` WHERE category='" + categoryshortname + "' ORDER by RAND() LIMIT 1";
            Console.WriteLine(q);
            //Create Command
            MySqlCommand cmd = new MySqlCommand(q, connection);
            
            //Create a data reader and Execute the command
            MySqlDataReader dataReader = cmd.ExecuteReader();
            Console.WriteLine(dataReader.FieldCount);
            
            //Read the data and store them in the list
            while (dataReader.Read())
            {
                Int32 id = dataReader.GetInt32("iddataitems");
                TimeSpan runtime = TimeSpan.FromMilliseconds(dataReader.GetInt32("runtime"));
                String label = dataReader.GetString("label");
                ret = new DBDataItem(label, id, runtime);



            }

            //close Data Reader
            dataReader.Close();

            return ret;
        }

        private int getCategoryIdFromShortName(string categoryshortname)
        {
            isConnected();
            String q = "SELECT id FROM `dataitems_category` WHERE shortname = '" + categoryshortname + "' LIMIT 0,1";
            Console.WriteLine(q);
            MySqlCommand cmd = new MySqlCommand(q, connection);
            Int32 res = Int32.Parse(cmd.ExecuteScalar().ToString());
            return res;
        }



        public LogList getLogList(DateTime date)
        {
            LogList log = new LogList();
            List<ScheduledSlice> scheduledslices =getScheduledSlices(date);

            for (int i = 0; i < scheduledslices.Count; i++)
            {
                ScheduledSlice scheduledslice = scheduledslices.ElementAt(i);
                LogListPrj.Slice slice = log.newSliceOnList(scheduledslice.scheduleddate);
                for (int j = 0; j < scheduledslice.slots.Count; j++)
                {
                    ScheduledSlot schslot = scheduledslice.slots.ElementAt(j);
                    DataItem item = null;
                    if (schslot.type == "AUDIO")
                    {
                        item = new DataSongItem(schslot.iddataitem, schslot.dataitem.artist, schslot.dataitem.title, System.Configuration.ConfigurationSettings.AppSettings["MusicPath"] + "" + schslot.dataitem.file, new TimeMarker(TimeSpan.Zero, schslot.dataitem.runtime));
                        item.setField("pic1", schslot.dataitem.radiovis1);
                        item.setField("pic2", schslot.dataitem.radiovis2);
                        item.setField("pic3", schslot.dataitem.radiovis3);
                        item.setField("pic4", schslot.dataitem.radiovis4);
                        item.setField("radiovistxt", schslot.dataitem.radiovistxt);
                        item.setField("currentdescr", schslot.dataitem.radiovistxt);

                    }
                    else if (schslot.type == "LOGNOTE")
                    {
                        item = new DataLogNoteItem(schslot.idhist, schslot.param, TimeSpan.Zero);

                    }
                    else if (schslot.type == "SYNC")
                    {
                        if(schslot.param == "HARD")
                            item = new DataSyncItem(schslot.idhist, DS.SyncType.HARD, schslot.scheduleddatetime);

                        else
                            item = new DataSyncItem(schslot.idhist, DS.SyncType.SOFT, schslot.scheduleddatetime);
                    }

                    if(item != null)
                        log.addDataToSlice(item, slice, schslot.idhist, date);
                }

            }

                log.refreshAirdatetime();
                log.loadPlaylist();
            return log;
        }

        private List<ScheduledSlice> getScheduledSlices(DateTime date)
        {

            isConnected();
            String q = "SELECT * FROM `scheduledslices` WHERE scheduleddatetime>='" + date.ToFileTime() + "' AND scheduleddatetime<'"+date.AddDays(1).ToFileTime()+"' ORDER by scheduleddatetime";
            Console.WriteLine(q);
            //Create Command
            MySqlCommand cmd = new MySqlCommand(q, connection);

            //Create a data reader and Execute the command
            MySqlDataReader dataReader = cmd.ExecuteReader();
            Console.WriteLine(dataReader.FieldCount);

            List<ScheduledSlice> scheduledslices = new List<ScheduledSlice>();

            //Read the data and store them in the list
            while (dataReader.Read())
            {
                Int32 idslice = dataReader.GetInt32("idscheduled");
                DateTime scheduleddatetime = DateTime.FromFileTime(dataReader.GetInt64("scheduleddatetime"));

                ScheduledSlice sl = new ScheduledSlice(idslice, scheduleddatetime);

                scheduledslices.Add(sl);


            }

            //close Data Reader
            dataReader.Close();

            for (int i = 0; i < scheduledslices.Count; i++)
            {
                scheduledslices.ElementAt(i).slots = getScheduledSlots(scheduledslices.ElementAt(i).id);
            }

            return scheduledslices;

        }

        private List<ScheduledSlot> getScheduledSlots(int idslice)
        {
            List<ScheduledSlot> scheduledslots = new List<ScheduledSlot>();
            isConnected();
            String q = "SELECT * FROM `scheduledslots` WHERE idscheduledslice = '" + idslice + "' ORDER by position";
            Console.WriteLine(q);
            //Create Command
            MySqlCommand cmd = new MySqlCommand(q, connection);

            //Create a data reader and Execute the command
            MySqlDataReader dataReader = cmd.ExecuteReader();

            

            //Read the data and store them in the list
            while (dataReader.Read())
            {
                Int32 idhist = dataReader.GetInt32("idscheduledslots");
                Int32 idscheduledslice = idslice;
                Int32 position = dataReader.GetInt32("position");
                String type = dataReader.GetString("type");
                Int32 iddataitem = dataReader.GetInt32("iddataitem");
                String param = dataReader.GetString("param");
                String label = dataReader.GetString("label");
                DateTime scheduleddatetime = DateTime.FromFileTime(dataReader.GetInt64("scheduleddatetime"));

                ScheduledSlot slot = new ScheduledSlot(idhist, idscheduledslice, position, type, iddataitem, param, label, scheduleddatetime);
                scheduledslots.Add(slot);

                


            }


            //close Data Reader
            dataReader.Close();
            for (int i = 0; i < scheduledslots.Count; i++)
            {
                if (scheduledslots.ElementAt(i).iddataitem != -1)
                    scheduledslots.ElementAt(i).dataitem = getDataItem(scheduledslots.ElementAt(i).iddataitem);
            }

            return scheduledslots;
        }

        private DBDataItem getDataItem(int iddataitem)
        {
            isConnected();
            String q = "SELECT * FROM `dataitems` WHERE iddataitems = '" + iddataitem + "'";
           // Console.WriteLine(q);
            //Create Command
            MySqlCommand cmd = new MySqlCommand(q, connection);

            //Create a data reader and Execute the command
            MySqlDataReader dataReader = cmd.ExecuteReader();



            //Read the data and store them in the list
                dataReader.Read();

                String label = dataReader.GetString("label");
                Int32 id = dataReader.GetInt32("iddataitems");
                String type = dataReader.GetString("type");
                TimeSpan runtime = TimeSpan.FromMilliseconds(dataReader.GetInt32("runtime"));
                String title = dataReader.GetString("title");
                String artist = dataReader.GetString("artist");
                String file = dataReader.GetString("file");
                String radiovis1 = dataReader.GetString("radiovis1");
                String radiovis2 = dataReader.GetString("radiovis2");
                String radiovis3 = dataReader.GetString("radiovis3");
                String radiovis4 = dataReader.GetString("radiovis3");
                String radiovistxt = dataReader.GetString("radiovistxt");
                String category = dataReader.GetString("category");

                dataReader.Close();
                DBDataItem item = new DBDataItem(label, id, runtime, type, title, artist,file,radiovis1, radiovis2, radiovis3, radiovis4, radiovistxt, category);
                return item;

        }



    }
    public class ScheduledSlice
    {
        public int id;
        public DateTime scheduleddate;
        public List<ScheduledSlot> slots = new List<ScheduledSlot>();
        public ScheduledSlice(int id, DateTime date)
        {
            this.id = id;
            this.scheduleddate = date;
            
        }

    }


    public class ScheduledSlot
    {
        public int idscheduledslice;
        public int position;
        public String type;
        public int iddataitem;
        public String param;
        public String label;
        public DateTime scheduleddatetime;
        public int idhist;
        public DBDataItem dataitem;

        public ScheduledSlot(int idhist, int idscheduledslice, int position, string type, int iddataitem, string param, string label, DateTime scheduleddatetime)
        {
            this.idhist = idhist;
            this.idscheduledslice = idscheduledslice;
            this.position = position;
            this.type = type;
            this.iddataitem = iddataitem;
            this.param = param;
            this.label = label;
            this.scheduleddatetime = scheduleddatetime;
            this.dataitem = null;
        }
        
    }
    public class DataItemCategory
    {
        public int id;
        public string name;
        public string shortname;
        public String categoryType;
        public DataItemCategory(int p, string p_2, string p_3, String type)
        {
            // TODO: Complete member initialization
            this.id = p;
            this.name = p_2;
            this.shortname = p_3;
            this.categoryType = type;
        }
    }
}
