using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using SlideGeneratorLib;
using System.Windows.Controls;
using System.Threading;
using ContentManagerService.Output.Stomp;
using ContentManagerService.Output.Ftp;
using System.Net;
using System.IO;
using Apache.NMS;
using Apache.NMS.Util;
using ContentManagerService.GUI;

namespace ContentManagerService
{
    public class BroadcastEngine
    {

        public int broadcastperiod { get; private set; } //in seconds
        private DispatcherTimer timer { get; set; }

        public List<String> slidecart { get; set; }
        public int slidePosition = 0;
        public SlideGenerator slidegen { get; set; }

        public String slideOnAir = "";

        private OutputFtp outputFtp;
        private OutputStomp outputStomp;


        public delegate void OutputEvent(String filename, String link);
        public event OutputEvent onBroadcast;

        public void setSlideCart(List<string> slidenames){
            this.slidecart.Clear();
            for(int i=0;i<slidenames.Count;i++){
                this.slidecart.Add(slidenames[i]);
            }
        }
        public BroadcastEngine(SlideGenerator slidegen)
        {
            this.slidegen = slidegen;
            this.slidecart = new List<string>();

            try
            {
                this.broadcastperiod = Int32.Parse(System.Configuration.ConfigurationSettings.AppSettings["broadcastdelay"]);
            }
            catch
            {
                this.broadcastperiod = 10;
            }
            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromSeconds(broadcastperiod);
            this.timer.Tick += new EventHandler(timer_Tick);

            this.outputStomp = new OutputStomp(this.slidegen);
            this.outputFtp = new OutputFtp();
            this.outputFtp.onUploadEnd += this.outputStomp.sendToStompProcess;
            this.outputFtp.onUploadEnd += new OutputFtp.OutputEvent(outputFtp_onUploadEnd);

            timer_Tick(this, new EventArgs());
        }

        void outputFtp_onUploadEnd(string filename, string link)
        {
            this.onBroadcast(filename, link);
        }
        int j = 0;
        int lastindex = 0;
        void timer_Tick(object sender, EventArgs e)
        {
            if (j == 0 && this.slidegen.cstlist.ContainsKey("BSLIDE") && this.slidegen.cstlist["BSLIDE"] != "" && (this.slidegen.cstlist["BSLIDE"]!= this.slideOnAir || this.slidecart.Count==0))
            {
                if(this.slidegen.cstlist["BSLIDE"]!= this.slideOnAir)
                    this.lastindex = this.slidecart.IndexOf(this.slideOnAir);
                broadcast(this.slidegen.cstlist["BSLIDE"]);
            }
            else
            {
                int i = this.slidecart.IndexOf(this.slideOnAir);
                if (i == -1) i = lastindex;
                if (i + 1 == this.slidecart.Count) i = 0;
                else i++;
                if (this.slidecart.Count > 0)
                    broadcast(this.slidecart.ElementAt(i));
            }
            j = (j + 1) % 5;
        }

        void start()
        {
            this.timer.Start();
        }

        void stop()
        {
            this.timer.Stop();
        }

        internal void broadcast(String slidename)
        {
            this.timer.Stop();
            this.timer.Start();
            
            this.slideOnAir = slidename;
            
            
            String filename = DateTime.Now.ToFileTimeUtc() + "-d-" + slidename + ".jpg";

            SlideResult slideres = slidegen.loadXMLSlide(slidename);
            if (slideres != null)
            {
                Canvas slide = slideres.image;
                String link = slideres.link;
                if (link == "")
                {
                    try
                    {
                        link = slidegen.cstlist["RADIOVISLINK"];
                    }
                    catch { }
                    if(link=="")
                        link = System.Configuration.ConfigurationSettings.AppSettings["radiovislink"];
                }
                int quality = Int32.Parse(System.Configuration.ConfigurationSettings.AppSettings["ftpImageQuality"]);

                slidegen.saveToJpg(slide, slidegen.tmpfolder + filename, link, quality);



                outputFtp.send(filename, slidegen.tmpfolder, link);

            }
            else
            {
                UIMain.errorAdd("[SLIDE] Not found or error when loading slide " + filename);
            }
            
           // this.onBroadcast(this, new EventBroadcastArgs(slidekey, DateTime.Now.ToLongTimeString()));

        }


        internal void startAutoBroadcast()
        {
            this.timer_Tick(null, new EventArgs());
            this.timer.Start();
        }
    }
}
