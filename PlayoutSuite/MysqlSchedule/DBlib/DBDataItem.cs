using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MysqlSchedule.DBlib
{
    public class DBDataItem
    {
        public int id;
        public String label;
        public TimeSpan runtime;
        public string type;
        public string title;
        public string artist;
        public string file;
        public string radiovis1;
        public string radiovis2;
        public string radiovis3;
        public string radiovis4;
        public string radiovistxt;
        public string category;


        public DBDataItem(String label, int id, TimeSpan runtime)
        {
            this.label = label;
            this.id = id;
            this.runtime = runtime;
        }

        public DBDataItem(string label, int id, TimeSpan runtime, string type, string title, string artist, string file, string radiovis1, string radiovis2, string radiovis3, string radiovis4, string radiovistxt, string category)
        {
            // TODO: Complete member initialization
            this.label = label;
            this.id = id;
            this.runtime = runtime;
            this.type = type;
            this.title = title;
            this.artist = artist;
            this.file = file;
            this.radiovis1 = radiovis1;
            this.radiovis2 = radiovis2;
            this.radiovis3 = radiovis3;
            this.radiovis4 = radiovis4;
            this.radiovistxt = radiovistxt;
            this.category = category;
        }
    }
}
