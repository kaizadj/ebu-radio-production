using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using ContentServiceLibrary;
using System.Windows.Threading;
using LogListPrj;
using LogListPrj.DataItems;
using System.Threading;
using System.ServiceModel;

namespace Playout.UI
{
    /// <summary>
    /// Interaction logic for PicFrame.xaml
    /// </summary>
    /// 


    [CallbackBehavior(ConcurrencyMode=ConcurrencyMode.Single)]
    public partial class PicFrame : UserControl, IContentServiceCallback
    {
        internal ContentServiceConnection srv;
        List<String> autobroadcastSlides;
        List<Image> UIimage = new List<Image>();
        String onairSlide;
        String tmpfolder ="-1";
        public PicFrame()
        {
            InitializeComponent();
            UIimage.Add(displayOA);
            UIimage.Add(display1);
            UIimage.Add(display2);


            srv = new ContentServiceConnection();
            this.connectToService();
                srv.channel.Faulted += new EventHandler(delegate(object o, EventArgs e)
                {
                    Console.WriteLine("[CONTENTSERVICE] Connection lost.");

                    MainWindow.errorAdd("Connection lost", "CONTENTSERVICE");
                    
              /*  this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                {
                    this.IsEnabled = false;
                }));*/
                });
            


            autobroadcastSlides = new List<string>();
           // autobroadcastSlides.Add("u2");
            //autobroadcastSlides.Add("demo");
            autobroadcastSlides.Add("ebu");
            autobroadcastSlides.Add("traffic");
            autobroadcastSlides.Add("song");

            refreshSlides();
        }

        internal void connectToService()
        {
            if (srv != null && (srv.channel == null || (srv.channel.State == System.ServiceModel.CommunicationState.Faulted || srv.channel.State == System.ServiceModel.CommunicationState.Closed)))
            {
                srv.connect("net.tcp://localhost:8095/ContentService", this);
                loadSlideCombo();
                refreshSlides();
            }
        }

        private void loadSlideCombo()
        {
            try
            {
                tmpfolder = srv.service.getTmpFolder();
            }
            catch
            {
                MainWindow.errorAdd("Unable to communicate with content service when sending var and getting slides", "CONTENTSERVICE");
            }
           /* try
            {
                List<String> slides = srv.service.getAvailableSlides();
                for (int i = 0; i < slides.Count; i++)
                {
                    this.comboTemplateSlides.Items.Add(slides[i]);
                }
            }
            catch
            {
                MainWindow.errorAdd("Unable to communicate with content service when sending var and getting slides", "CONTENTSERVICE");
            }*/

        }

        public void assignNewOnAirHandler(PlayOutFrame frame)
        {
            frame.newOnAirStatus += new PlayOutFrame.newOnAirStatusHandler(frame_newOnAirStatus);
            
        }

        void frame_newOnAirStatus(object sender, OnairEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("NEW ON AIR SLOT : "+e.onairslot.item.label);
            if(e!=null && e.onairslot != null && e.onairslot.item.dataitemtype == DS.DataItemType.LOGNOTE)
                this.newAutoBroadcastContentFromLog(e.onairslot.item.label);

            Console.ForegroundColor = ConsoleColor.White;
                    Thread newThread =
                    new Thread(new ThreadStart(this.refreshSlides));
                    newThread.Start();
        }

        public void refreshLabel()
        {
         /*   this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                try
                {
                    MainWindow main = ((MainWindow)((Grid)((Grid)this.Parent).Parent).Parent);
                    DateTime airdate = main.playoutFrame.CurrentSlot.slice.node.Next.Value.airdatetime;
                    DateTime scheduleddate = main.playoutFrame.CurrentSlot.slice.node.Next.Value.scheduleddatetime;
                    TimeSpan gaptime = scheduleddate.Subtract(airdate);
                    this.labelNextSync.Content = "Next Sync: " + airdate + " (gap: " + gaptime+")";
                }
                catch {
                    this.labelNextSync.Content = "";
                }
            }));
            */
                                


        }

        public void refreshSlides(){
            refreshLabel();
            if (srv.service != null && srv.channel.State != System.ServiceModel.CommunicationState.Faulted)
            {
                try
                {
                    SlotItem onairslot = ((MainWindow)((Grid)((Grid)this.Parent).Parent).Parent).playoutFrame.CurrentSlot;
                    //Console.WriteLine("preview demo");
                    if (onairslot != null && onairslot.item.dataitemtype == DS.DataItemType.SONG)
                    {
                        DataSongItem i = (DataSongItem)onairslot.item;

                        srv.service.setVar("currentsong", i.title);
                        srv.service.setVar("currentartist", i.artist);
                        srv.service.setVar("currentsongid", i.ID.ToString());
                        srv.service.setVar("currentpic1", i.getField("pic1"));
                        srv.service.setVar("currentpic2", i.getField("pic2"));
                        srv.service.setVar("currentpic3", i.getField("pic3"));
                        srv.service.setVar("currentpic4", i.getField("pic4"));
                        Console.WriteLine("******##############\n" + i.getField("radiovistxt"));
                        srv.service.setVar("currentdescr", i.getField("radiovistxt"));
                        
                    }
                    else
                    {
                        srv.service.setVar("currentsong", "Only MUSIC");
                        srv.service.setVar("currentartist", "by EBU Radio");
                        
                    }

                    List<String> li = new List<String>();
                    for (int i = 0; i < this.UIimage.Count; i++)
                    {
                        if (i < autobroadcastSlides.Count)
                        {
                            String url=null;
                            try
                            {
                                url = srv.service.getPreview(this.autobroadcastSlides.ElementAt(i));
                            }
                            catch { Console.WriteLine("error preview slide : " + (this.autobroadcastSlides[i])); }
                            if (url != null)
                            {
                                this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                                {

                                    BitmapImage im = null;

                                    try
                                    {
                                        im = imgFromFile(tmpfolder + "" + url);
                                    }
                                    catch
                                    {
                                        // MessageBox.Show("ERROR when generating files:" + tmpfolder + " ...");
                                        MainWindow.errorAdd("Error when loading files in " + tmpfolder + " ...", "CONTENTSERVICE");
                                    }

                                    if (im != null)
                                    {
                                        UIimage[i].Source = im;
                                        UIimage[i].Tag = this.autobroadcastSlides.ElementAt(i);
                                    }

                                }));

                                li.Add(this.autobroadcastSlides.ElementAt(i));
                            }
                        }
                        else
                        {
                            this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                            {
                                UIimage[i].Source = null;
                            }));
                        }
                    }
                    this.srv.service.setAutoBroadcastParameters(TimeSpan.FromSeconds(10), li);
                    this.srv.service.setAutoBroadcast(true);
                    /*
                    Thread newThread = new Thread(new ParameterizedThreadStart(this.sendBroadcastCommand));
                    newThread.Start(this.autobroadcastSlides[0]);
                     */
                }
                catch
                {
                    MainWindow.errorAdd("Unable to communicate with content service when sending var and getting slides", "CONTENTSERVICE");
                }
            }
            else
            {

                MainWindow.errorAdd("Not connected...", "CONTENTSERVICE");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(new ThreadStart(takePicWebcam));
            t.Start();
        }


        private void takePicWebcam()
        {
            String webcamfile = System.Configuration.ConfigurationSettings.AppSettings["WebcamTmpFolder"] + DateTime.Now.ToFileTime() + ".jpg";
            Boolean saved = false;
            String webcamtext = "";
            this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                if (player.CurrentBitmap != null)
                {
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    BitmapFrame outputFrame = BitmapFrame.Create(player.CurrentBitmap);
                    encoder.Frames.Add(outputFrame);
                    encoder.QualityLevel = 100;
                    try
                    {
                        using (FileStream file = File.OpenWrite(webcamfile))
                        {
                            encoder.Save(file);
                            file.Close();
                            saved = true;
                        }
                        webcamtext = this.webcamText.Text;
                    }
                    catch(Exception e)
                    {
                        MainWindow.errorAdd("Error : "+e.Message, "CONTENTSERVICE");
                    }
                    
                }
                else
                {
                    MainWindow.errorAdd("WEBCAM - try Again", "CONTENTSERVICE");
                }
            }));

            if (saved)
            {
                try
                {
                    srv.service.setVar("WEBCAMPIC", webcamfile);
                    srv.service.setVar("WEBCAMTXT", webcamtext);
                    srv.service.broadcast("webcam");
                }
                catch (Exception er)
                {
                    MainWindow.errorAdd("Unable to broadcast slide: " + er.Message, "CONTENTSERVICE");
                }
            }
            
        }


        private BitmapImage imgFromFile(String path){
            BitmapImage bi = new BitmapImage();
            // BitmapImage.UriSource must be in a BeginInit/EndInit block.
            bi.BeginInit();
            bi.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
            bi.EndInit();
            return bi;
        }

        

        public void OnUpdate(string name, string value)
        {
            throw new NotImplementedException();
        }

        private void display_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            AutoCloseWindow w = new AutoCloseWindow(((Image)sender).Source);
            w.Show();
        }


        public void OnOnAirChange(string onairslidekey)
        {
            Console.WriteLine("****************** PIC CHANGE : " + onairslidekey + " " + DateTime.Now.ToLongTimeString());

            if (onairslidekey != null)
            {
                this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                {
                    try
                    {
                        for (int i = 0; i < UIimage.Count; i++)
                        {
                            UIimage[i].Opacity = (UIimage[i].Tag.ToString().Equals(onairslidekey)) ? 1.0 : 0.5; ;
                        }
                    }
                    catch { }
                }));

                
            }

        }

        private void display_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            String slidekey = ((Image)sender).Tag.ToString();
            for (int i = 0; i < UIimage.Count; i++)
            {
                UIimage[i].Opacity = 0.5;
            }
            /*try
            {

                this.srv.service.broadcast(slidekey);
            }
            catch
            {
                this.srv.channel.Abort();
            }*/

            Thread newThread = new Thread(new ParameterizedThreadStart(this.sendBroadcastCommand));
            newThread.Start(slidekey);
        }
        
        private void sendBroadcastCommand(object slidekey){
            try
            {
                this.srv.service.broadcast((String)slidekey);
            }
            catch
            {
                try
                {
                    this.srv.channel.Abort();
                }
                catch
                {

                    MainWindow.errorAdd("Unable to abort", "CONTENTSERVICE");
                }
                MainWindow.errorAdd("Unable to broadcast slide", "CONTENTSERVICE");
            }
        }

        private void btn_changeSLIDES_Click(object sender, RoutedEventArgs e)
        {
            this.newAutoBroadcastContentFromLog("@@@SLIDESLOAD-onair|u2");
        }

        public void newAutoBroadcastContentFromLog(String logloadslide)
        {

            if (logloadslide.StartsWith("@@@SLIDESLOAD-"))
            {
                String slides = logloadslide.Substring(("@@@SLIDESLOAD-").Length);
               


                List<String> li = slides.Split(new char[] {'|'}).ToList();
                /*li.Add(this.autobroadcastSlides.ElementAt(0));
                li.Add(this.autobroadcastSlides.ElementAt(1));
                li.Add(this.autobroadcastSlides.ElementAt(2));*/
                try
                {
                    this.autobroadcastSlides = li;
                    Console.WriteLine("CHANGE SLIDE PANEL : " +slides);
                    Console.WriteLine("SLIDE PANEL CHANGED");
                }
                catch
                {
                    try
                    {
                        this.srv.channel.Abort();
                    }
                    catch
                    {

                        MainWindow.errorAdd("Unable to abort", "CONTENTSERVICE");
                    }
                    MainWindow.errorAdd("Unable to change broadcast content", "CONTENTSERVICE");
                }


            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            Thread t = new Thread(new ThreadStart(takePicWebcam));
            t.Start();
        }

    }
}
