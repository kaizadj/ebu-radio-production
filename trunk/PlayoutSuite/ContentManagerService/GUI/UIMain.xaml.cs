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
using SlideGeneratorLib;
using System.Threading;
using ContentServiceLibrary;
using System.ServiceModel;
using System.Windows.Threading;
using System.IO;
using System.Deployment.Application;

namespace ContentManagerService.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class UIMain : Window
    {
        
       
        public static ContentManagerCore core;
        public static UIMain Instance;

        public UIMain()
        {
            System.Diagnostics.Debugger.Launch();
            core = ContentManagerCore.getInstance();
            InitializeComponent();
            Instance = this;


            core.engine.onBroadcast += new BroadcastEngine.OutputEvent(engine_onBroadcast);
        }

        void engine_onBroadcast(string filename, string link)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                //errorlist.Items.Add("onair : " + autobroadcast.slideOnair);
                // Canvas s = slidegen.loadXMLSlide(autobroadcast.slideOnair);
                Image s = new Image();
                s.Source = loadImage(filename);
                if (s != null)
                    panel2.Children.Add(s);

                if (panel2.Children.Count > 1)
                    panel2.Children.RemoveAt(0);
            }));
        }

        private BitmapImage loadImage(String filename)
        {
            FileStream f = new FileStream(core.slidegen.tmpfolder + "" + filename, FileMode.Open);
            FileStream f2 = new FileStream(core.slidegen.tmpfolder + "tmp" + filename, FileMode.CreateNew);
            f.CopyTo(f2);
            f.Close();
            f2.Close();

            BitmapImage myBitmapImage = new BitmapImage();
            myBitmapImage.BeginInit();
            myBitmapImage.UriSource = new Uri(core.slidegen.tmpfolder+"tmp"+filename);
            myBitmapImage.EndInit();
            return myBitmapImage;
        }

        /*  void autobroadcast_onBroadcast(object sender, EventArgs e)
          {
                     this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                 {
                     errorlist.Items.Add("onair : " + autobroadcast.slideOnair);
                     // Canvas s = slidegen.loadXMLSlide(autobroadcast.slideOnair);
                      if (s != null)
                          panel2.Children.Add(s);

                      if (panel2.Children.Count > 1)
                          panel2.Children.RemoveAt(0);
                 }));
          }
          */
        public static void errorAdd(String msg, String module = "undefined")
        {
            if (Instance != null)
            {
                errorListAdd(msg, module);
            }
        }
        private static void errorListAdd(String msg, String module = "undefined")
        {
            if (Instance != null)
            {
                Instance.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                {
                    Instance.errorlist.Items.Add("[" + module + "] " + DateTime.Now.ToLongTimeString() + ": " + msg);
                }));
            }
        }
        public static void fatalError(String msg)
        {
            MessageBox.Show(msg, "FATAL ERROR");
            Environment.Exit(1);            
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {

            SlideResult res = core.slidegen.loadXMLSlide(textBox1.Text);
            if (res != null)
            {
                Canvas s = res.image;
                if (s != null)
                    panel.Children.Add(s);

                if (panel.Children.Count > 1)
                    panel.Children.RemoveAt(0);
            }
            
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String str = "";
            str += "Variables :\n";
            for (int i = 0; i < core.slidegen.cstlist.Count; i++)
            {
                str += core.slidegen.cstlist.ElementAt(i).Key + "=" +core.slidegen.cstlist.ElementAt(i).Value;
                str += "\n";
            }
            MessageBox.Show(str);
        }

        private void BClick_debug(object sender, RoutedEventArgs e)
        {
            errorlist.Visibility = (errorlist.Visibility == System.Windows.Visibility.Hidden) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            core.test();
            core.broadcast("u2");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            String str = "";
            str += "Slides :\n";
            List<String> slidenames = core.slidegen.getAvailableSlides();
            
            for (int i = 0; i < slidenames.Count; i++)
            {
                str += slidenames[i];
                str += "\n";
            }
            MessageBox.Show(str);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(core.datafolder);

        }


    }
}
