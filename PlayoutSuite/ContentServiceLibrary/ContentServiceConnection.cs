using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace ContentServiceLibrary
{
    public class ContentServiceConnection
    {
        public IContentService service = null;
        public ICommunicationObject channel;
        private DuplexChannelFactory<IContentService> duplex = null;

        public void connect(String url, IContentServiceCallback callback, EventHandler openedEvt = null, EventHandler faultEvt = null) //url = "net.tcp://localhost:8080/AudioService"
        {
            try
            {
                duplex = new DuplexChannelFactory<IContentService>(callback, new NetTcpBinding(),
                    new EndpointAddress(url));
                
                service = duplex.CreateChannel();
                channel = (ICommunicationObject)service;
                IClientChannel c = (IClientChannel)channel;
                
                c.OperationTimeout = TimeSpan.FromSeconds(5);
                
                channel.Opened += new EventHandler(delegate(object o, EventArgs e)
                {
                    Console.WriteLine("[CONTENTSERVICE] Connection ok!");
                });

                if (openedEvt != null)
                    channel.Opened += openedEvt;
                if (faultEvt != null)
                    channel.Faulted += faultEvt;
                channel.Faulted += new EventHandler(delegate(object o, EventArgs e)
                {
                    Console.WriteLine("[CONTENTSERVICE] Connection lost");
                });

                channel.Closed += new EventHandler(delegate(object o, EventArgs e)
                    {
                        Console.WriteLine("[CONTENTSERVICE] Connection closed");
                    });

            }
            catch (Exception e)
            {
                Console.WriteLine("[CONTENTSERVICE] Connection error: " + e.Message);
            }
        }

        public void disconnect()
        {
            ((ICommunicationObject)service).Close();
            duplex.Close();
        }

        
    }
}
