using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using ContentServiceLibrary;


namespace ContentManagerService
{
    /// <summary>
    /// InputWCF is used for synchronization with EBU Radio Production Platform
    /// </summary>
    class InputWCF : IInputPlugin
    {
        public static ServiceHost host;
        public static String url = "net.tcp://localhost:8095/ContentService";
        private ContentManagerCore contentManagerCore;

        public InputWCF(){}

        public InputWCF(ContentManagerCore contentManagerCore)
        {
            this.contentManagerCore = contentManagerCore;
        }

        void host_Faulted(object sender, EventArgs e)
        {
            Console.WriteLine("Connection error");
        }

        void host_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("connection ok!");
        }

        public void setup(Dictionary<string, string> variableList)
        {
            //variableList not used directly
            //this.url ......
            
        }

        public bool start()
        {
            Console.WriteLine("Opening ContentEngineService...");
            Type serviceType = typeof(IContentService);
            /*
            try
            {
                host = new ServiceHost(typeof(ContentService));
                host.Opened += new EventHandler(host_Opened);
                host.Faulted += new EventHandler(host_Faulted);
                host.AddServiceEndpoint(typeof(IContentService), new NetTcpBinding(), url);
                host.Open();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("WCF: Error when opening socket\n"+e.Message);
                return false;
            }*/
            return false;
            
        }

        public bool stop()
        {
            try
            {
                host.Close();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        public string getPluginType()
        {
            return "WCF";
        }
    }
}
