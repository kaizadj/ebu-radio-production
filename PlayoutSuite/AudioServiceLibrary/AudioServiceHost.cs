using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using AudioEngineDll;

namespace AudioServiceLibrary
{
    public class AudioServiceHost
    {
        ServiceHost duplex;
        public static IAudioManager audiomgr = null;

        public AudioServiceHost(IAudioManager audiomanager)
        {
            audiomgr = audiomanager;
            
            
        }
        public void open(String url, EventHandler OpenedEvt=null)//"net.tcp://localhost:8080/AudioService"
        {
            Type serviceType = typeof(IAudioService);

            //ServiceHost duplex = new ServiceHost(typeof(AudioService), new Uri[] { new Uri("net.tcp://localhost:8090/"), new Uri("http://localhost:8080/") });
            //duplex = new ServiceHost(typeof(AudioService));
            duplex = new ServiceHost(typeof(AudioService));
            if (OpenedEvt != null)
                duplex.Opened += OpenedEvt;
            
            duplex.AddServiceEndpoint(typeof(IAudioService), new NetTcpBinding(), url);

            duplex.Open();
        }
    }

    
    
}
