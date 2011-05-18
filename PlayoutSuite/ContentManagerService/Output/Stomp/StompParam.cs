using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContentManagerService.Output.Stomp
{
    class StompParam
    {
        public String address;
        public int port;
        public String topic;
        public String showparam;
        public String link;

        public StompParam(String address, int port, string topic, string link, string showparam)
        {
            this.address = address;
            this.port = port;
            this.topic = topic;
            this.link = link;
            this.showparam = showparam;
        }
    }
}
