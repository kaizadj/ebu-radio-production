using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MysqlSchedule.DBlib
{
    public class DBfield
    {
        public int id {  get; private set; }
        public string name{  get; private set;}

        public DBfield(int id, String name)
        {
            this.id = id;
            this.name = name;
        }

        public DBfield getFieldFromId(int szid)
        {
            DBengineMySql dbengine = DBengineMySql.GetInstance();
            return dbengine.getField(szid);
        }

        public DBfield getFieldFromName(string szname)
        {
            DBengineMySql dbengine = DBengineMySql.GetInstance();
            return dbengine.getField(szname);

        }
        public override string ToString()
        {
            return name + " (" + id + ")";
        }
        public override bool Equals(object obj)
        {
            DBfield o = (DBfield)obj;

            return o.id == this.id;
        }
        public override int GetHashCode()
        {
            return this.id;
        }
    }
}
