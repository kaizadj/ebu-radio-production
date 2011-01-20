using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace AudioServiceLibrary
{
    public class AudioServiceConnection
    {
        
        public IAudioService service=null;
        public ICommunicationObject channel;
        private DuplexChannelFactory<IAudioService> duplex = null;

        public void connect(String url, IAudioServiceCallBack callback, EventHandler openedEvt = null, EventHandler faultEvt = null) //url = "net.tcp://localhost:8080/AudioService"
        {
            try
            {
                 duplex = new DuplexChannelFactory<IAudioService>(callback, new NetTcpBinding(),
                     new EndpointAddress(url));
                
                 service = duplex.CreateChannel();
                 channel = (ICommunicationObject)service;
                 IClientChannel c = (IClientChannel)channel;
                 c.OperationTimeout = TimeSpan.FromSeconds(5);
                 
                 channel.Opened += new EventHandler(delegate(object o, EventArgs e)
                    {
                        Console.WriteLine("Connection ok!");
                    });

                if(openedEvt != null)
                 channel.Opened += openedEvt;
                if(faultEvt != null)
                 channel.Faulted += faultEvt;
                 channel.Faulted += new EventHandler(delegate(object o, EventArgs e)
                    {
                        Console.WriteLine("Connection lost");
                    });

            }
            catch (Exception e)
            {
                Console.WriteLine("Connection error: " + e.Message);
            }
        }

        public void disconnect()
        {
            ((ICommunicationObject)service).Close();
            duplex.Close();
        }


        public void hello(string name, string key)
        {
            this.service.hello(name, key);
        }
    }
}
