using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlideGeneratorLib;
using ContentManagerService.Input.HTTP;
using System.Windows;
using ContentManagerService.GUI;
using System.Deployment.Application;
using System.IO;

namespace ContentManagerService
{
    /// <summary>
    /// SINGLETON
    /// 
    /// </summary>
    public class ContentManagerCore
    {
        private static ContentManagerCore instance = null;
        public SlideGenerator slidegen;
        public BroadcastEngine engine;
        public String datafolder;
        
        public IInputPlugin input;
        
        //Output


        private static Object InstanceLock = new Object();
        
        private ContentManagerCore() {
            
            datafolder = System.Configuration.ConfigurationSettings.AppSettings["DataFolder"];
            
            if (!Directory.Exists(datafolder) && !File.Exists(datafolder + "\\conf.xml") && ApplicationDeployment.IsNetworkDeployed)
                datafolder = ApplicationDeployment.CurrentDeployment.DataDirectory + "\\" + datafolder;

            this.slidegen = new SlideGenerator(datafolder + "\\conf.xml");
            
            if (System.Configuration.ConfigurationSettings.AppSettings["InputMethod"] == "HTTP")
                this.input = new InputHTTP(this);
            else if (System.Configuration.ConfigurationSettings.AppSettings["InputMethod"] == "WCF")
                this.input = new InputWCF(this);
            else
            {
                MessageBox.Show("There is no InputMethod key in the configuration file", "Error: Bad Configuration");
            }
            this.engine = new BroadcastEngine(slidegen);
            this.engine.setSlideCart(this.getAutoSlides());
            this.engine.startAutoBroadcast();
            
        }

        public static ContentManagerCore getInstance()
        {
            lock (InstanceLock)
            {
                if (instance == null)
                {
                    instance = new ContentManagerCore();
                }
                return instance;
            }
        }

        internal void test()
        {
            /*SlideResult slideres = this.slidegen.loadXMLSlide("u2");
            if(slideres != null)
            this.slidegen.saveToJpg(slideres.image, "test.jpg", true);*/
        }

        internal void update(Dictionary<string, string> dictionary)
        {
            //Lock for enumeration thread safety
            lock (slidegen.cstlist)
            {
                if (dictionary != null)
                {
                    for (int i = 0; i < dictionary.Count; i++)
                    {
                        if (slidegen.cstlist.ContainsKey(dictionary.ElementAt(i).Key))
                        {
                            slidegen.cstlist[dictionary.ElementAt(i).Key] = dictionary.ElementAt(i).Value;
                            if(dictionary.ElementAt(i).Key =="CURRENTARTIST")
                                slidegen.cstlist["ARTIST"] = dictionary.ElementAt(i).Value;
                            if (dictionary.ElementAt(i).Key == "CURRENTTITLE")
                                slidegen.cstlist["TITLE"] = dictionary.ElementAt(i).Value;
                        }
                        else
                        {
                            slidegen.cstlist.Add(dictionary.ElementAt(i).Key, dictionary.ElementAt(i).Value);

                            if (dictionary.ElementAt(i).Key == "CURRENTARTIST")
                                slidegen.cstlist.Add("ARTIST", dictionary.ElementAt(i).Value);
                            if (dictionary.ElementAt(i).Key == "CURRENTTITLE")
                                slidegen.cstlist.Add("TITLE", dictionary.ElementAt(i).Value);
                        }
                    }
                }
            }
        }

        internal void broadcast(String slidename)
        {
            UIMain.Instance.Dispatcher.Invoke((Action)delegate() { this.engine.broadcast(slidename); });
        }


        private List<String> getAutoSlides()
        {
            List<String> slides = new List<String>();
            List<String> availableSlides = this.slidegen.getAvailableSlides();
            for (int i = 0; i < availableSlides.Count; i++)
            {
                String slidename = availableSlides.ElementAt(i);
                if (slidename.IndexOf(System.Configuration.ConfigurationSettings.AppSettings["slidecartprefix"]) == 0)
                {
                    slides.Add(slidename);
                }

            }
            return slides;
        }
    }
}
