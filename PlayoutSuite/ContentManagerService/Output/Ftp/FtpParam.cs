using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContentManagerService.Output.Ftp
{
    public class FtpParam
    {
        public string address;
        public string user;
        public string password;
        public string filename;
        public string alone;
        public int minperiod; //seconds
        public int id;
        public string link;

        public FtpParam(string address, string user, string password, string filename, string link, string alone="no", string minperiod="0", int id=-1)
        {
            this.address = address;
            this.user = user;
            this.password = password;
            this.filename = filename;
            this.alone = alone;
            this.link = link;
            try
            {
                this.minperiod = Int32.Parse(minperiod);
            }
            catch
            {
                this.minperiod = 0;
            }
            this.id = id;
        }
    }
}
