using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContentManagerService.Input.HTTP
{
    public class InputHTTPAction
    {
        ContentManagerCore core;
        public InputHTTPAction(ContentManagerCore core)
        {
            this.core = core;
        }

        internal void update(Dictionary<string, string> dictionary)
        {
            core.update(dictionary);
        }

        internal void updateAndBroadcast(Dictionary<string, string> dictionary, String broadcastslidename)
        {
            core.update(dictionary);
            core.broadcast(broadcastslidename);
        }

        internal void broadcast(String broadcastslide)
        {
            core.broadcast(broadcastslide);
        }
        internal void loadslidecart(Dictionary<string, string> dictionary)
        {
            core.engine.slidecart.Clear();

            List<String> l = new List<String>();
            foreach(KeyValuePair<String,String> t in dictionary){
                if(t.Key.StartsWith("SLIDE"))
                    l.Add(t.Value);
            }
            core.engine.setSlideCart(l);
        }
    }
}
