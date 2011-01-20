using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MysqlSchedule.DBlib
{
    public class DBEventArgs:EventArgs
    {
        public Dictionary<DBfield, string> values = null;
        public DBEventArgs(Dictionary<DBfield, String> values)
            : base()
        {
            this.values = values;
        }
    }
}
