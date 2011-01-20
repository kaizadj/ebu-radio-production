using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContentServiceLibrary;
using System.Windows.Controls;
using System.ServiceModel;

namespace ContentManagerService
{
    
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class ContentService : IContentService
    {
        public IContentServiceCallback callback = null;
        public IContextChannel channel = null;

        public ContentService()
        {
            this.callback = OperationContext.Current.GetCallbackChannel<IContentServiceCallback>();
            MainWindow.autobroadcast.onBroadcast += new EventHandler(autobroadcast_onBroadcast);
            channel = OperationContext.Current.Channel;
        }

        void autobroadcast_onBroadcast(object sender, EventArgs e)
        {
            EventBroadcastArgs b = (EventBroadcastArgs)e;
         //   Console.WriteLine("[CONTENTSERVICE] ON BROADCAST > CALLBACK :"+DateTime.Now.ToLongTimeString()+" "+b.slide);
            try
            {
                if (channel.State == CommunicationState.Opened)
                    callback.OnOnAirChange(b.slide);

            }
            catch
            {
                MainWindow.errorAdd("Callback aborted"+ channel.State);

            }
        }
            

        public List<string> getAvailableSlides()
        {
            return MainWindow.slidegen.getAvailableSlides();
        }

        public List<string> getAvailableSlides(string prefix)
        {

            return MainWindow.slidegen.getAvailableSlides(prefix);
        }

        public String broadcast(string slidekey)
        {
            Console.WriteLine(" BROADCAST " + slidekey + "!");
            MainWindow.autobroadcast.broadcast(slidekey);
            return "";
        }

        public String getPreview(string slidekey)
        {

            Console.WriteLine(" PREVIEW " + slidekey + "!");
            //MainWindow.slidegen.
            Canvas c = MainWindow.slidegen.loadXMLSlide(slidekey);
            if (c == null) return null;
            String url=""+slidekey+"-"+DateTime.Now.ToFileTime()+".jpg";
            MainWindow.slidegen.saveToJpg(c, MainWindow.slidegen.tmpfolder+url);
            return url;
        }


        public void setVar(string key, string content)
        {
            MainWindow.slidegen.setVar(key, content);
        }

        public string getVar(string key)
        {
            return MainWindow.slidegen.getVar(key);
        }


        public void setAutoBroadcast(bool isEnabled)
        {
            MainWindow.autobroadcast.setAutoBroadcast(isEnabled);
            
        }

        public bool getAutoBroadCastEnabled()
        {
            return MainWindow.autobroadcast.getAutoBroadcast();
        }

        public void setAutoBroadcastParameters(TimeSpan Interval, List<string> slidekeys)
        {
            
            MainWindow.autobroadcast.setAutoBroadcastParameters(Interval, slidekeys);
        }

        public List<string> getAutoBroadcastSlides()
        {
            return MainWindow.autobroadcast.getAutoBroadCastSlides();
        }

        public TimeSpan getAutoBroadcastInterval()
        {
            return MainWindow.autobroadcast.getAutoBroadcastInterval();
        }
        public String getTmpFolder()
        {
            return MainWindow.slidegen.tmpfolder;
        }

        
            
    }
}
